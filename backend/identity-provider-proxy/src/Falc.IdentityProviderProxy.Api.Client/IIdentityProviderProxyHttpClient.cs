using Falc.IdentityProviderProxy.Api.Client.Contracts;

namespace Falc.IdentityProviderProxy.Api.Client;

public interface IIdentityProviderProxyHttpClient
{
    Task<HttpResponseMessage> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken);
}
