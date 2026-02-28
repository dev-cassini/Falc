using System.Net.Http.Json;
using Falc.Communications.Api.Client.Contracts;

namespace Falc.Communications.Api.Client;

public sealed class CommunicationsHttpClient(HttpClient httpClient) : ICommunicationsHttpClient
{
    private const string BaseRoute = "api/communications";
    
    public async Task<HttpResponseMessage> SearchUsersAsync(SearchUsersRequest request, CancellationToken cancellationToken)
    {
        return await httpClient.PostAsJsonAsync($"{BaseRoute}/users/search", request, cancellationToken);
    }
}
