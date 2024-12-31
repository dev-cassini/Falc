using Microsoft.Extensions.DependencyInjection;

namespace Falc.Communications.Infrastructure.MediatR;

internal static class Extensions
{
    internal static IServiceCollection AddMediatR(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssemblyContaining<Application.Marker>();
        });
        
        return serviceCollection;
    }
}