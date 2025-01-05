namespace Falc.Communications.Application.Queries;

public static class GetUser
{
    public record MarketingPreferencesDto(bool Email, bool Phone, bool Sms);
    
    public record Dto(Guid Id, string Email, MarketingPreferencesDto MarketingPreferences);
    
    public interface IQueryHandler
    {
        Task<Dto?> ExecuteAsync(Guid userId, CancellationToken cancellationToken);
    }
}