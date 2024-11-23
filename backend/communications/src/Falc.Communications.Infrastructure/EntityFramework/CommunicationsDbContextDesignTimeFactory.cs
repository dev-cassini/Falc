using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Falc.Communications.Infrastructure.EntityFramework;

public class CommunicationsDbContextDesignTimeFactory : IDesignTimeDbContextFactory<CommunicationsDbContext>
{
    public CommunicationsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CommunicationsDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Username=postgres;Password=pass;Database=Falc.Platform;Include Error Detail=true");

        return new CommunicationsDbContext(optionsBuilder.Options);
    }
}