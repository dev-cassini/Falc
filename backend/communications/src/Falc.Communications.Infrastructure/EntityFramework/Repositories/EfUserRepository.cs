using Falc.Communications.Domain.Model;
using Falc.Communications.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Falc.Communications.Infrastructure.EntityFramework.Repositories;

public class EfUserRepository(CommunicationsDbContext dbContext) : IUserRepository
{
    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await dbContext.Users.AddAsync(user, cancellationToken);
    }

    public async Task<User?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Users.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}