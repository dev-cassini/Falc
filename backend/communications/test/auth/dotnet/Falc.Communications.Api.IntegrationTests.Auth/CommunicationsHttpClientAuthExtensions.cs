using Falc.Communications.Api.Client;
using Falc.Communications.Api.Client.Contracts;

namespace Falc.Communications.Api.IntegrationTests.Auth;

public static class CommunicationsHttpClientAuthExtensions
{
    public static async Task<CommunicationsHttpClient> AuthenticateAsAdmin(this CommunicationsHttpClient client, CancellationToken cancellationToken = default)
    {
        var token = await AuthIntegrationTestContext.TokenClient.GetAdminTokenAsync(cancellationToken);
        client.WithBearerToken(token);
        return client;
    }

    public static async Task<CommunicationsHttpClient> AuthenticateAsNonAdmin(this CommunicationsHttpClient client, CancellationToken cancellationToken = default)
    {
        var token = await AuthIntegrationTestContext.TokenClient.GetNonAdminTokenAsync(cancellationToken);
        client.WithBearerToken(token);
        return client;
    }

    public static async Task<HttpResponseMessage> SearchUsersAsync(
        this Task<CommunicationsHttpClient> clientTask,
        SearchUsersRequest request,
        CancellationToken cancellationToken)
    {
        var client = await clientTask.ConfigureAwait(false);
        return await client.SearchUsersAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
