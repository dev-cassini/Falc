using Falc.Communications.Application.Queries;
using Falc.Communications.Domain.Repositories;
using Falc.Communications.Infrastructure.EntityFramework.Queries;
using Falc.Communications.Infrastructure.EntityFramework.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Falc.Communications.Infrastructure.EntityFramework;

internal static class Extensions
{
    internal static IServiceCollection AddEntityFramework(
        this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        serviceCollection
            .AddDbContext<CommunicationsDbContext>(builder =>
            {
                var connectionString = configuration.GetConnectionString("Postgres");
                builder.UseNpgsql(connectionString, optionsBuilder =>
                {
                    optionsBuilder.MigrationsHistoryTable("__EFMigrationsHistory", CommunicationsDbContext.Schema);
                });
            })
            .AddEfRepositories()
            .AddEfQueries();

        return serviceCollection;
    }

    internal static IServiceProvider MigrateDatabase(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CommunicationsDbContext>();
        dbContext.Database.Migrate();

        return serviceProvider;
    }

    private static IServiceCollection AddEfRepositories(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddScoped<IUserRepository, EfUserRepository>();

        return serviceCollection;
    }

    private static IServiceCollection AddEfQueries(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddScoped<GetUser.IQueryHandler, EfGetUserQueryHandler>()
            .AddScoped<SearchUsers.IQueryHandler, EfSearchUsersQueryHandler>();

        return serviceCollection;
    }
}
