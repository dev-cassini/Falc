using System.Diagnostics;
using System.ComponentModel;
using System.Text.Json;

namespace Falc.Communications.Api.IntegrationTests.Auth;

public static class SettingsLoader
{
    public static async Task<AuthIntegrationTestSettings> LoadAsync(CancellationToken cancellationToken = default)
    {
        var environmentName = Environment.GetEnvironmentVariable("FALC_AUTH_ENV_NAME") ?? "dev";
        var secretId = Environment.GetEnvironmentVariable("FALC_AUTH_AWS_SECRET_ID") ?? $"falc/communications/auth-tests/{environmentName}";
        var region = Environment.GetEnvironmentVariable("FALC_AUTH_AWS_REGION") ?? "eu-west-2";
        var profile = Environment.GetEnvironmentVariable("FALC_AUTH_AWS_PROFILE");

        var source = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        var awsSettings = await LoadFromAwsSecretsManagerAsync(secretId, region, profile, cancellationToken);
        foreach (var (key, value) in awsSettings)
        {
            source[key] = value;
        }

        foreach (var (key, value) in LoadFromEnvironmentVariables())
        {
            source[key] = value;
        }

        foreach (var (key, value) in LoadFromLocalFile())
        {
            source[key] = value;
        }

        var mergedSettings = new AuthIntegrationTestSettings
        {
            BaseUrl = GetRequired(source, "baseUrl"),
            IdpTokenUrl = GetRequired(source, "idpTokenUrl"),
            IdpClientId = GetRequired(source, "idpClientId"),
            IdpClientSecret = GetRequired(source, "idpClientSecret"),
            IdpImpersonationGrantType = GetOrDefault(source, "idpImpersonationGrantType", "urn:ietf:params:oauth:grant-type:token-exchange"),
            IdpImpersonationFieldName = GetOrDefault(source, "idpImpersonationFieldName", "subject"),
            AdminUserIdentifier = GetRequired(source, "adminUserIdentifier"),
            NonAdminUserIdentifier = GetRequired(source, "nonAdminUserIdentifier"),
            RequiredRole = GetOrDefault(source, "requiredRole", "admin"),
            TimeoutSeconds = GetIntOrDefault(source, "timeoutSeconds", 30),
            EnvironmentName = GetOrDefault(source, "environmentName", environmentName),
            AwsSecretId = GetOrDefault(source, "awsSecretId", secretId),
            AwsRegion = GetOrDefault(source, "awsRegion", region),
            AwsProfile = GetOrDefault(source, "awsProfile", profile ?? string.Empty)
        };

        mergedSettings.Validate();
        return mergedSettings;
    }

    private static Dictionary<string, string> LoadFromEnvironmentVariables()
    {
        var env = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        AddIfPresent(env, "baseUrl", "FALC_AUTH_BASE_URL");
        AddIfPresent(env, "idpTokenUrl", "FALC_AUTH_IDP_TOKEN_URL");
        AddIfPresent(env, "idpClientId", "FALC_AUTH_IDP_CLIENT_ID");
        AddIfPresent(env, "idpClientSecret", "FALC_AUTH_IDP_CLIENT_SECRET");
        AddIfPresent(env, "idpImpersonationGrantType", "FALC_AUTH_IDP_IMPERSONATION_GRANT_TYPE");
        AddIfPresent(env, "idpImpersonationFieldName", "FALC_AUTH_IDP_IMPERSONATION_FIELD_NAME");
        AddIfPresent(env, "adminUserIdentifier", "FALC_AUTH_ADMIN_USER_IDENTIFIER");
        AddIfPresent(env, "nonAdminUserIdentifier", "FALC_AUTH_NON_ADMIN_USER_IDENTIFIER");
        AddIfPresent(env, "requiredRole", "FALC_AUTH_REQUIRED_ROLE");
        AddIfPresent(env, "timeoutSeconds", "FALC_AUTH_TIMEOUT_SECONDS");
        AddIfPresent(env, "environmentName", "FALC_AUTH_ENV_NAME");
        AddIfPresent(env, "awsSecretId", "FALC_AUTH_AWS_SECRET_ID");
        AddIfPresent(env, "awsRegion", "FALC_AUTH_AWS_REGION");
        AddIfPresent(env, "awsProfile", "FALC_AUTH_AWS_PROFILE");

        return env;
    }

    private static Dictionary<string, string> LoadFromLocalFile()
    {
        var localPathFromEnv = Environment.GetEnvironmentVariable("FALC_AUTH_LOCAL_SETTINGS_PATH");
        var candidatePaths = new[]
        {
            localPathFromEnv,
            Path.Combine(Directory.GetCurrentDirectory(), "appsettings.local.json"),
            Path.Combine(GetProjectDirectory(), "appsettings.local.json")
        }.Where(path => !string.IsNullOrWhiteSpace(path)).Cast<string>().Distinct().ToList();

        foreach (var path in candidatePaths)
        {
            if (!File.Exists(path))
            {
                continue;
            }

            var json = File.ReadAllText(path);
            return ParseFlatObjectJson(json, $"local settings file at '{path}'");
        }

        return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }

    private static async Task<Dictionary<string, string>> LoadFromAwsSecretsManagerAsync(
        string secretId,
        string region,
        string? profile,
        CancellationToken cancellationToken)
    {
        var command = new List<string>
        {
            "secretsmanager",
            "get-secret-value",
            "--secret-id", secretId,
            "--region", region,
            "--query", "SecretString",
            "--output", "text"
        };

        if (!string.IsNullOrWhiteSpace(profile))
        {
            command.AddRange(["--profile", profile]);
        }

        var processStartInfo = new ProcessStartInfo
        {
            FileName = "aws",
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        foreach (var token in command)
        {
            processStartInfo.ArgumentList.Add(token);
        }

        try
        {
            using var process = new Process { StartInfo = processStartInfo };
            process.Start();

            var stdoutTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
            var stderrTask = process.StandardError.ReadToEndAsync(cancellationToken);
            await process.WaitForExitAsync(cancellationToken);

            var stdout = await stdoutTask;
            var stderr = await stderrTask;

            if (process.ExitCode != 0)
            {
                throw new InvalidOperationException(
                    $"Unable to read auth test settings from AWS Secrets Manager secret '{secretId}'. aws exit code: {process.ExitCode}. stderr: {stderr}");
            }

            if (string.IsNullOrWhiteSpace(stdout))
            {
                throw new InvalidOperationException(
                    $"AWS Secrets Manager secret '{secretId}' returned an empty SecretString.");
            }

            return ParseFlatObjectJson(stdout, $"AWS secret '{secretId}'");
        }
        catch (Win32Exception exception)
        {
            throw new InvalidOperationException(
                "AWS CLI is required to load auth integration settings. Install AWS CLI or set environment variables/local settings file.",
                exception);
        }
    }

    private static Dictionary<string, string> ParseFlatObjectJson(string json, string sourceDescription)
    {
        using var document = JsonDocument.Parse(json);
        if (document.RootElement.ValueKind != JsonValueKind.Object)
        {
            throw new InvalidOperationException($"Settings from {sourceDescription} must be a JSON object.");
        }

        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var property in document.RootElement.EnumerateObject())
        {
            values[property.Name] = property.Value.ValueKind switch
            {
                JsonValueKind.String => property.Value.GetString() ?? string.Empty,
                JsonValueKind.Number => property.Value.GetRawText(),
                JsonValueKind.True => bool.TrueString,
                JsonValueKind.False => bool.FalseString,
                _ => property.Value.GetRawText()
            };
        }

        return values;
    }

    private static string GetRequired(IDictionary<string, string> source, string key)
    {
        if (source.TryGetValue(key, out var value) && string.IsNullOrWhiteSpace(value) is false)
        {
            return value;
        }

        throw new InvalidOperationException($"Required auth integration setting '{key}' is missing.");
    }

    private static string GetOrDefault(IDictionary<string, string> source, string key, string defaultValue)
    {
        return source.TryGetValue(key, out var value) && string.IsNullOrWhiteSpace(value) is false
            ? value
            : defaultValue;
    }

    private static int GetIntOrDefault(IDictionary<string, string> source, string key, int defaultValue)
    {
        return source.TryGetValue(key, out var value) && int.TryParse(value, out var parsed)
            ? parsed
            : defaultValue;
    }

    private static string GetProjectDirectory()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (directory.EnumerateFiles("*.csproj").Any())
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        return Directory.GetCurrentDirectory();
    }

    private static void AddIfPresent(IDictionary<string, string> target, string key, string environmentVariable)
    {
        var value = Environment.GetEnvironmentVariable(environmentVariable);
        if (!string.IsNullOrWhiteSpace(value))
        {
            target[key] = value;
        }
    }
}
