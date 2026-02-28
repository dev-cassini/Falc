using System.Text.Json;

namespace Falc.Communications.Api.IntegrationTests.Auth;

public sealed class AuthIntegrationTestSettings
{
    public required string BaseUrl { get; init; }
    public required string IdpTokenUrl { get; init; }
    public required string IdpClientId { get; init; }
    public required string IdpClientSecret { get; init; }
    public string IdpImpersonationGrantType { get; init; } = "urn:ietf:params:oauth:grant-type:token-exchange";
    public string IdpImpersonationFieldName { get; init; } = "subject";
    public required string AdminUserIdentifier { get; init; }
    public required string NonAdminUserIdentifier { get; init; }
    public string RequiredRole { get; init; } = "admin";
    public int TimeoutSeconds { get; init; } = 30;
    public string EnvironmentName { get; init; } = "dev";
    public string? AwsSecretId { get; init; }
    public string? AwsRegion { get; init; }
    public string? AwsProfile { get; init; }

    public static AuthIntegrationTestSettings FromJson(string json)
    {
        var settings = JsonSerializer.Deserialize<AuthIntegrationTestSettings>(
            json,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));

        if (settings is null)
        {
            throw new InvalidOperationException("Auth integration test settings JSON is empty or invalid.");
        }

        settings.Validate();
        return settings;
    }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(BaseUrl))
        {
            throw new InvalidOperationException("BaseUrl is required.");
        }

        if (string.IsNullOrWhiteSpace(IdpTokenUrl))
        {
            throw new InvalidOperationException("IdpTokenUrl is required.");
        }

        if (string.IsNullOrWhiteSpace(IdpClientId))
        {
            throw new InvalidOperationException("IdpClientId is required.");
        }

        if (string.IsNullOrWhiteSpace(IdpClientSecret))
        {
            throw new InvalidOperationException("IdpClientSecret is required.");
        }

        if (string.IsNullOrWhiteSpace(AdminUserIdentifier))
        {
            throw new InvalidOperationException("AdminUserIdentifier is required.");
        }

        if (string.IsNullOrWhiteSpace(NonAdminUserIdentifier))
        {
            throw new InvalidOperationException("NonAdminUserIdentifier is required.");
        }
    }
}
