namespace Falc.Communications.Api.Endpoints;

public static class Extensions
{
    public static WebApplication RegisterEndpoints(this WebApplication webApplication)
    {
        webApplication.UsePathBase("/api/communications");
        
        webApplication
            .RegisterGetMyUserEndpoint()
            .RegisterPatchMyUserEndpoint()
            .RegisterGetUserEndpoint()
            .RegisterPatchUserEndpoint()
            .RegisterSearchUsersEndpoint();

        return webApplication;
    }
}
