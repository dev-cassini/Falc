using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Falc.Communications.Api.IntegrationTests.Auth;

public sealed class TokenClient(HttpClient httpClient, AuthIntegrationTestSettings settings)
{
    public async Task<string> GetAdminTokenAsync(CancellationToken cancellationToken)
    {
        return await GetTokenAsync(settings.AdminUserIdentifier, cancellationToken);
    }

    public async Task<string> GetNonAdminTokenAsync(CancellationToken cancellationToken)
    {
        return await GetTokenAsync(settings.NonAdminUserIdentifier, cancellationToken);
    }

    private async Task<string> GetTokenAsync(string userIdentifier, CancellationToken cancellationToken)
    {
        var request = new Dictionary<string, string>
        {
            ["grant_type"] = settings.IdpImpersonationGrantType,
            ["client_id"] = settings.IdpClientId,
            ["client_secret"] = settings.IdpClientSecret,
            [settings.IdpImpersonationFieldName] = userIdentifier
        };

        using var response = await httpClient.PostAsync(
            settings.IdpTokenUrl,
            new FormUrlEncodedContent(request),
            cancellationToken);

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Failed to fetch token for user '{userIdentifier}'. Status={(int)response.StatusCode}, Body={content}");
        }

        var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken: cancellationToken);
        if (string.IsNullOrWhiteSpace(tokenResponse?.AccessToken))
        {
            throw new InvalidOperationException("IdP token response did not include access_token.");
        }

        return tokenResponse.AccessToken;
    }

    public static void SetBearerToken(HttpClient client, string? token)
    {
        client.DefaultRequestHeaders.Authorization = string.IsNullOrWhiteSpace(token)
            ? null
            : new AuthenticationHeaderValue("Bearer", token);
    }

    private sealed record TokenResponse(string access_token)
    {
        public string AccessToken => access_token;
    }
}
