namespace Falc.IdentityProvider.Api.Proxy.Endpoints;

public static class Extensions
{
    public static WebApplication RegisterEndpoints(this WebApplication webApplication)
    {
        webApplication
            .RegisterCreateUserEndpoint();

        return webApplication;
    }
}
