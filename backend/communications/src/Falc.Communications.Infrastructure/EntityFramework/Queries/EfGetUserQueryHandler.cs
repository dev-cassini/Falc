using Falc.Communications.Application.Queries;
using Microsoft.EntityFrameworkCore;

namespace Falc.Communications.Infrastructure.EntityFramework.Queries;

public class EfGetUserQueryHandler(CommunicationsDbContext dbContext) : GetUser.IQueryHandler
{
    public async Task<GetUser.Dto?> ExecuteAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await dbContext.Users
            .Where(x => x.Id == userId)
            .Select(x => new GetUser.Dto(
                x.Id, 
                x.EmailAddress,
                new GetUser.MarketingPreferencesDto(
                    x.MarketingPreferences.Email, 
                    x.MarketingPreferences.Phone,
                    x.MarketingPreferences.Sms)))
            .SingleOrDefaultAsync(cancellationToken);
    }
}