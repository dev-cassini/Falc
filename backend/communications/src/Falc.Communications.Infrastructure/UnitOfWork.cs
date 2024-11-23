using Falc.Communications.Application;
using Falc.Communications.Infrastructure.EntityFramework;

namespace Falc.Communications.Infrastructure;

public class UnitOfWork(CommunicationsDbContext dbContext) : IUnitOfWork
{
    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}