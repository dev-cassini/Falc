using Falc.Communications.Infrastructure.EntityFramework;
using Falc.Communications.Infrastructure.MassTransit;
using Falc.Communications.Infrastructure.MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Falc.Communications.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        serviceCollection
            .AddEntityFramework(configuration)
            .AddMassTransit(configuration)
            .AddMediatR();

        return serviceCollection;
    }

    public static IServiceProvider UseInfrastructure(this IServiceProvider serviceProvider)
    {
        serviceProvider.MigrateDatabase();

        return serviceProvider;
    }
}