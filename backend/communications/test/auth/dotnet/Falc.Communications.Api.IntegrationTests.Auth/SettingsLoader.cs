using System.Text.Json;

namespace Falc.Communications.Api.IntegrationTests.Auth;

public static class SettingsLoader
{
    public static Task<AuthIntegrationTestSettings> LoadAsync()
    {
        var source = LoadFromLocalFile();

        var mergedSettings = new AuthIntegrationTestSettings
        {
            BaseUrl = GetRequired(source, "baseUrl"),
            IdpTokenUrl = GetRequired(source, "idpTokenUrl"),
            IdpClientId = GetRequired(source, "idpClientId"),
            IdpClientSecret = GetRequired(source, "idpClientSecret"),
            IdpDelegationPasswordGrantUsername = GetOrDefault(
                source,
                "idpDelegationPasswordGrantUsername",
                GetOrDefault(source, "idpDelegationUsername", "admin")),
            IdpDelegationPasswordGrantPassword = GetOrDefault(
                source,
                "idpDelegationPasswordGrantPassword",
                GetOrDefault(source, "idpDelegationPassword", "Pa$$word123")),
            IdpPasswordGrantScope = GetOrDefault(source, "idpPasswordGrantScope", "openid profile email"),
            IdpImpersonationGrantType = GetOrDefault(source, "idpImpersonationGrantType", "delegation"),
            IdpImpersonationFieldName = GetOrDefault(source, "idpImpersonationFieldName", "subject"),
            AdminUserIdentifier = GetRequired(source, "adminUserIdentifier"),
            CustomerUserIdentifier = GetRequired(source, source.ContainsKey("customerUserIdentifier")
                ? "customerUserIdentifier"
                : "nonAdminUserIdentifier"),
            TimeoutSeconds = GetIntOrDefault(source, "timeoutSeconds", 30),
            EnvironmentName = GetOrDefault(source, "environmentName", "dev"),
        };

        mergedSettings.Validate();
        return Task.FromResult(mergedSettings);
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

        throw new InvalidOperationException(
            "No local auth integration settings file was found. Set FALC_AUTH_LOCAL_SETTINGS_PATH or place appsettings.local.json in the project folder.");
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
}
