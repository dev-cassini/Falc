using Falc.Communications.Application.Commands;
using Falc.Communications.Domain.Model;

namespace Falc.Communications.Api.Endpoints;

public static class PatchMyUserEndpoint
{
    public static WebApplication RegisterPatchMyUserEndpoint(this WebApplication webApplication)
    {
        webApplication
            .MapPatch("users/me", Handler)
            .RequireAuthorization(builder => builder.RequireAuthenticatedUser())
            .WithTags(nameof(User))
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);
            
        return webApplication;
    }
    
    private static async Task<IResult> Handler(
        PatchUser.Command request,
        MediatR.ISender sender,
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
        
        await sender.Send(request, cancellationToken);
        return Results.NoContent();
    }
}