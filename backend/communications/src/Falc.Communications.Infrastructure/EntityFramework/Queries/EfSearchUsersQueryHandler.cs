using Falc.Communications.Application.Queries;
using Microsoft.EntityFrameworkCore;

namespace Falc.Communications.Infrastructure.EntityFramework.Queries;

public class EfSearchUsersQueryHandler(CommunicationsDbContext dbContext) : SearchUsers.IQueryHandler
{
    public async Task<SearchUsers.Result> ExecuteAsync(SearchUsers.Request request, CancellationToken cancellationToken)
    {
        var normalizedEmailAddress = request.EmailAddress.Trim().ToLowerInvariant();
        var query = dbContext.Users
            .AsNoTracking()
            .Where(x => x.EmailAddress.Contains(normalizedEmailAddress));

        var totalCount = await query.CountAsync(cancellationToken);

        var users = await query
            .OrderBy(x => x.EmailAddress)
            .ThenBy(x => x.Id)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new SearchUsers.UserDto(
                x.Id,
                x.EmailAddress,
                new SearchUsers.MarketingPreferencesDto(
                    x.MarketingPreferences.Email,
                    x.MarketingPreferences.Phone,
                    x.MarketingPreferences.Sms)))
            .ToListAsync(cancellationToken);

        return new SearchUsers.Result(users, request.PageNumber, request.PageSize, totalCount);
    }
}
