using Falc.Communications.Domain.Model;
using Falc.Communications.Domain.Repositories;
using Falc.Communications.Domain.Tooling.Abstractions;
using MediatR;

namespace Falc.Communications.Application.Commands;

public static class CreateUser
{
    public record Command(Guid Id, string EmailAddress) : IRequest;
    
    public class CommandHandler(
        IUserRepository userRepository,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork) 
        : IRequestHandler<Command>
    {
        public async Task Handle(Command command, CancellationToken cancellationToken)
        {
            var user = new User(command.Id, command.EmailAddress, dateTimeProvider);
            await userRepository.AddAsync(user, cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);
        }
    }
}