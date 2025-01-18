using Falc.Communications.Application.Queries;
using Falc.Communications.Domain.Model;

namespace Falc.Communications.Api.Endpoints;

public static class GetMyUserEndpoint
{
    public static WebApplication RegisterGetMyUserEndpoint(this WebApplication webApplication)
    {
        webApplication
            .MapGet("users/me", Handler)
            .RequireAuthorization(builder => builder.RequireAuthenticatedUser())
            .WithTags(nameof(User))
            .Produces(StatusCodes.Status200OK, typeof(GetUser.Dto))
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);
            
            return webApplication;
    }

    private static async Task<IResult> Handler(
        GetUser.IQueryHandler getUserQueryHandler,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var sub = httpContext.User.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;
        if (sub is null)
        {
            return Results.NotFound("User sub claim not found.");
        }
        
        if (Guid.TryParse(sub, out var userId) is false)
        {
            return Results.NotFound($"User sub claim value is not a guid: {sub}");
        }
        
        var user = await getUserQueryHandler.ExecuteAsync(userId, cancellationToken);
        return user is null 
            ? Results.NotFound($"User with id {userId} not found.") 
            : Results.Ok(user);
    }
}