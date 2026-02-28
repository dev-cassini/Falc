using Falc.Communications.Api.Client.Contracts;

namespace Falc.Communications.Api.Client;

public interface ICommunicationsHttpClient
{
    Task<HttpResponseMessage> SearchUsersAsync(SearchUsersRequest request, CancellationToken cancellationToken);
}
