namespace Falc.Communications.Application;

public interface IUnitOfWork
{
    Task CommitAsync(CancellationToken cancellationToken);
}