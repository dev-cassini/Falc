using Falc.Communications.Domain.Repositories;

namespace Falc.Communications.Application.Commands;

public static class PatchUser
{
    public record MarketingPreferencesDto(bool Email, bool Phone, bool Sms);
    public record Command(Guid Id, MarketingPreferencesDto MarketingPreferences);

    public interface ICommandHandler
    {
        Task ExecuteAsync(Command command, CancellationToken cancellationToken);
    }

    public static class Exceptions
    {
        public class UserNotFoundException(Guid id) : Exception($"User {id} not found.");
    }
    
    public class CommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork) 
        : ICommandHandler
    {
        public async Task ExecuteAsync(Command command, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetAsync(command.Id, cancellationToken);
            if (user is null)
            {
                throw new Exceptions.UserNotFoundException(command.Id);
            }
            
            user.MarketingPreferences.Email = command.MarketingPreferences.Email;
            user.MarketingPreferences.Phone = command.MarketingPreferences.Phone;
            user.MarketingPreferences.Sms = command.MarketingPreferences.Sms;
            
            await unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
