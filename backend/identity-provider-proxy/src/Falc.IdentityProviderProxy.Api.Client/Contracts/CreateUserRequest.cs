namespace Falc.IdentityProviderProxy.Api.Client.Contracts;

public sealed record CreateUserRequest(
    string Id,
    string UserName,
    string Email,
    bool EmailConfirmed,
    string PhoneNumber,
    bool PhoneNumberConfirmed,
    bool LockoutEnabled,
    bool TwoFactorEnabled,
    int AccessFailedCount,
    DateTimeOffset LockoutEnd);
