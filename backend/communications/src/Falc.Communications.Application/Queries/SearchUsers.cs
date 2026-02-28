namespace Falc.Communications.Application.Queries;

public static class SearchUsers
{
    public record Request(string EmailAddress, int PageNumber, int PageSize);
    
    public record MarketingPreferencesDto(bool Email, bool Phone, bool Sms);

    public record UserDto(Guid Id, string Email, MarketingPreferencesDto MarketingPreferences);

    public record Result(IReadOnlyCollection<UserDto> Users, int PageNumber, int PageSize, int TotalCount);

    public interface IQueryHandler
    {
        Task<Result> ExecuteAsync(Request request, CancellationToken cancellationToken);
    }
}
