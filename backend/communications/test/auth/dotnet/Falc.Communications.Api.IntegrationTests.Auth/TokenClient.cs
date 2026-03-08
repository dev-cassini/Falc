using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Falc.Communications.Api.IntegrationTests.Auth;

public sealed class TokenClient(HttpClient httpClient, AuthIntegrationTestSettings settings)
{
    private readonly SemaphoreSlim _adminTokenLock = new(1, 1);
    private readonly SemaphoreSlim _customerTokenLock = new(1, 1);
    private string? _adminToken;
    private string? _customerToken;

    public async Task<string> GetAdminTokenAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_adminToken) is false)
        {
            return _adminToken;
        }

        await _adminTokenLock.WaitAsync(cancellationToken);
        try
        {
            if (string.IsNullOrWhiteSpace(_adminToken))
            {
                _adminToken = await GetTokenAsync(settings.AdminUserIdentifier, cancellationToken);
            }

            return _adminToken!;
        }
        finally
        {
            _adminTokenLock.Release();
        }
    }

    public async Task<string> GetCustomerTokenAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_customerToken) is false)
        {
            return _customerToken;
        }

        await _customerTokenLock.WaitAsync(cancellationToken);
        try
        {
            if (string.IsNullOrWhiteSpace(_customerToken))
            {
                _customerToken = await GetTokenAsync(settings.CustomerUserIdentifier, cancellationToken);
            }

            return _customerToken!;
        }
        finally
        {
            _customerTokenLock.Release();
        }
    }

    private async Task<string> GetTokenAsync(string userIdentifier, CancellationToken cancellationToken)
    {
        if (string.Equals(
                settings.IdpImpersonationGrantType,
                "delegation",
                StringComparison.OrdinalIgnoreCase))
        {
            var sourceToken = await GetPasswordGrantTokenAsync(cancellationToken);
            return await GetDelegationTokenAsync(sourceToken, userIdentifier, cancellationToken);
        }

        return await GetTokenExchangeTokenAsync(userIdentifier, cancellationToken);
    }

    private async Task<string> GetTokenExchangeTokenAsync(string userIdentifier, CancellationToken cancellationToken)
    {
        var request = new Dictionary<string, string>
        {
            ["grant_type"] = settings.IdpImpersonationGrantType,
            ["client_id"] = settings.IdpClientId,
            ["client_secret"] = settings.IdpClientSecret,
            [settings.IdpImpersonationFieldName] = userIdentifier
        };

        return await SendTokenRequestAsync(request, userIdentifier, cancellationToken);
    }

    private async Task<string> GetDelegationTokenAsync(string sourceToken, string userIdentifier, CancellationToken cancellationToken)
    {
        var request = new Dictionary<string, string>
        {
            ["grant_type"] = settings.IdpImpersonationGrantType,
            ["client_id"] = settings.IdpClientId,
            ["client_secret"] = settings.IdpClientSecret,
            ["token"] = sourceToken
        };

        if (string.IsNullOrWhiteSpace(settings.IdpImpersonationFieldName) is false)
        {
            request[settings.IdpImpersonationFieldName] = userIdentifier;
        }

        return await SendTokenRequestAsync(request, userIdentifier, cancellationToken);
    }

    private async Task<string> GetPasswordGrantTokenAsync(CancellationToken cancellationToken)
    {
        var request = new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["client_id"] = settings.IdpClientId,
            ["client_secret"] = settings.IdpClientSecret,
            ["username"] = settings.IdpDelegationPasswordGrantUsername,
            ["password"] = settings.IdpDelegationPasswordGrantPassword
        };

        if (string.IsNullOrWhiteSpace(settings.IdpPasswordGrantScope) is false)
        {
            request["scope"] = settings.IdpPasswordGrantScope;
        }

        using var response = await httpClient.PostAsync(
            settings.IdpTokenUrl,
            new FormUrlEncodedContent(request),
            cancellationToken);

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Failed to fetch delegation source token. Status={(int)response.StatusCode}, Body={content}");
        }

        var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken: cancellationToken);
        if (string.IsNullOrWhiteSpace(tokenResponse?.AccessToken))
        {
            throw new InvalidOperationException("IdP password grant response did not include access_token.");
        }

        return tokenResponse.AccessToken;
    }

    private async Task<string> SendTokenRequestAsync(
        Dictionary<string, string> request,
        string userIdentifier,
        CancellationToken cancellationToken)
    {
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
