namespace Falc.Communications.Api.Client.Contracts;

public sealed record SearchUsersRequest(string? EmailAddress = null, int PageNumber = 1, int PageSize = 25);
