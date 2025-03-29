namespace Falc.RmqFacade.Api.Endpoints;

public static class Extensions
{
    public static WebApplication RegisterEndpoints(this WebApplication webApplication)
    {
        webApplication.UsePathBase("/api/rmq-facade");

        webApplication
            .RegisterGetQueuesEndpoint();
        
        return webApplication;
    }
}