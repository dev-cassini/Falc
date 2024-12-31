using Falc.Communications.Domain.Tooling;
using Falc.Communications.Domain.Tooling.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Falc.Communications.Domain;

public static class Extensions
{
    public static IServiceCollection AddDomainTooling(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IDateTimeProvider, DefaultDateTimeProvider>();
        return serviceCollection;
    }
}