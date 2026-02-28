using System.Net.Http.Json;
using Falc.IdentityProviderProxy.Api.Client.Contracts;

namespace Falc.IdentityProviderProxy.Api.Client;

public sealed class IdentityProviderProxyHttpClient(HttpClient httpClient) : IIdentityProviderProxyHttpClient
{
    private const string BaseRoute = "api/idp-proxy";
    
    public async Task<HttpResponseMessage> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken)
    {
        return await httpClient.PostAsJsonAsync($"{BaseRoute}/users", request, cancellationToken);
    }
}
