using Falc.IdentityProvider.Api.Proxy.Clients.IdentityProvider.Models;

namespace Falc.IdentityProvider.Api.Proxy.Clients.IdentityProvider;

public interface IIdentityProviderHttpClient
{
    Task<HttpResponseMessage> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken);
}
