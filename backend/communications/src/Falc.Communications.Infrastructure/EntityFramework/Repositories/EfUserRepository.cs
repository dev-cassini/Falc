using Falc.Communications.Domain.Model;
using Falc.Communications.Domain.Repositories;

namespace Falc.Communications.Infrastructure.EntityFramework.Repositories;

public class EfUserRepository(CommunicationsDbContext dbContext) : IUserRepository
{
    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await dbContext.Users.AddAsync(user, cancellationToken);
    }
}