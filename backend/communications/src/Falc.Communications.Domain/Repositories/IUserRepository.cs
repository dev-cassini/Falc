using Falc.Communications.Domain.Model;

namespace Falc.Communications.Domain.Repositories;

public interface IUserRepository
{
    Task AddAsync(User user, CancellationToken cancellationToken);
}