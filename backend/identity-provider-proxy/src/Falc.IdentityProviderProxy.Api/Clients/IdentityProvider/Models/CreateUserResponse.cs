namespace Falc.IdentityProvider.Api.Proxy.Clients.IdentityProvider.Models;

public sealed record CreateUserResponse(
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
