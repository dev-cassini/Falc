namespace Falc.Communications.Api.Client.Contracts;

public sealed record SearchUsersResponse(
    IReadOnlyCollection<SearchUsersResponseUser> Users,
    int PageNumber,
    int PageSize,
    int TotalCount);

public sealed record SearchUsersResponseUser(
    Guid Id,
    string Email,
    SearchUsersResponseMarketingPreferences MarketingPreferences);

public sealed record SearchUsersResponseMarketingPreferences(
    bool Email,
    bool Phone,
    bool Sms);
