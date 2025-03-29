namespace Falc.RmqFacade.Api.Services;

public static class Extensions
{
    public static IServiceCollection AddServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHttpClient<IRmqManagementApi, RmqManagementApi>((serviceProvider, httpClient) =>
        {
            var rmqConnectionString = serviceProvider.GetRequiredService<IConfiguration>().GetConnectionString("Rmq");
            httpClient.BaseAddress = new Uri(rmqConnectionString ?? string.Empty);
        });
        
        return serviceCollection;
    }
}