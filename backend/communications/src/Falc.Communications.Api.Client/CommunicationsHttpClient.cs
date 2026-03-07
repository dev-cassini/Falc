using System.Net.Http.Json;
using System.Net.Http.Headers;
using Falc.Communications.Api.Client.Contracts;

namespace Falc.Communications.Api.Client;

public sealed class CommunicationsHttpClient(HttpClient httpClient) : ICommunicationsHttpClient
{
    private const string BaseRoute = "api/communications";
    
    public ICommunicationsHttpClient WithBearerToken(string? token)
    {
        httpClient.DefaultRequestHeaders.Authorization = token is null
            ? null
            : new AuthenticationHeaderValue("Bearer", token);

        return this;
    }

    public async Task<HttpResponseMessage> SearchUsersAsync(SearchUsersRequest request, CancellationToken cancellationToken)
    {
        return await httpClient.PostAsJsonAsync($"{BaseRoute}/users/search", request, cancellationToken);
    }
}
