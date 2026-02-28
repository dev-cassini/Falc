using System.Net.Http.Json;
using Falc.IdentityProvider.Api.Proxy.Clients.IdentityProvider.Models;

namespace Falc.IdentityProvider.Api.Proxy.Clients.IdentityProvider;

public sealed class IdentityProviderHttpClient(HttpClient httpClient) : IIdentityProviderHttpClient
{
    public async Task<HttpResponseMessage> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync("/api/Users", request, cancellationToken);
        return response;
    }
}
