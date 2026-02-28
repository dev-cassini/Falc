namespace Falc.IdentityProvider.Api.Proxy.Clients.IdentityProvider;

public static class Extensions
{
    public static IServiceCollection AddIdentityProviderHttpClient(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddHttpClient<IIdentityProviderHttpClient, IdentityProviderHttpClient>();

        return serviceCollection;
    }
}
