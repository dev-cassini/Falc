using System.Reflection;
using Falc.Communications.Infrastructure.EntityFramework;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Falc.Communications.Infrastructure.MassTransit;

internal static class Extensions
{
    internal static IServiceCollection AddMassTransit(
        this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        serviceCollection
            .AddMassTransit(configurator =>
            {
                configurator.UsingRabbitMq((context, factoryConfigurator) =>
                {
                    var rmqUri = configuration.GetConnectionString("Rmq");
                    factoryConfigurator.Host(rmqUri);
                    factoryConfigurator.ConfigureEndpoints(context, new CustomEndpointNameFormatter());
                });
                
                configurator.AddEntityFrameworkOutbox<CommunicationsDbContext>(outboxConfigurator =>
                {
                    outboxConfigurator
                        .UsePostgres()
                        .UseBusOutbox();
                });
                
                configurator.AddConsumers(Assembly.GetExecutingAssembly());
            });
        
        return serviceCollection;
    }

    internal static ModelBuilder AddMassTransitOutbox(this ModelBuilder modelBuilder)
    {
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();

        return modelBuilder;
    }
}