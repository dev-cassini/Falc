using Falc.Communications.Domain.Model;
using Falc.Communications.Domain.Repositories;
using Falc.Communications.Domain.Tooling.Abstractions;

namespace Falc.Communications.Application.Commands;

public static class CreateUser
{
    public record Command(Guid Id, string EmailAddress);

    public interface ICommandHandler
    {
        Task ExecuteAsync(Command command, CancellationToken cancellationToken);
    }
    
    public class CommandHandler(
        IUserRepository userRepository,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork) 
        : ICommandHandler
    {
        public async Task ExecuteAsync(Command command, CancellationToken cancellationToken)
        {
            var user = new User(command.Id, command.EmailAddress, dateTimeProvider);
            await userRepository.AddAsync(user, cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);
        }
    }
}