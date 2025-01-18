using Falc.Communications.Api.Authentication.Schemes;
using Falc.Communications.Application.Queries;
using Falc.Communications.Domain.Model;
using Microsoft.AspNetCore.Authorization;

namespace Falc.Communications.Api.Endpoints;

public static class GetUserEndpoint
{
    public static WebApplication RegisterGetUserEndpoint(this WebApplication webApplication)
    {
        webApplication
            .MapGet("users/{userId}", Handler)
            .RequireAuthorization()
            .WithTags(nameof(User))
            .Produces(StatusCodes.Status200OK, typeof(GetUser.Dto))
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);
            
        return webApplication;
    }

    [Authorize(AuthenticationSchemes = HmacAuthenticationScheme.Name)]
    private static async Task<IResult> Handler(
        Guid userId,
        GetUser.IQueryHandler getUserQueryHandler,
        CancellationToken cancellationToken)
    {
        var user = await getUserQueryHandler.ExecuteAsync(userId, cancellationToken);
        return user is null 
            ? Results.NotFound($"User with id {userId} not found.") 
            : Results.Ok(user);
    }
}