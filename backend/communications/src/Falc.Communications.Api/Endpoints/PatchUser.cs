using Falc.Communications.Api.Authentication.Schemes;
using Falc.Communications.Application.Commands;
using Falc.Communications.Domain.Model;
using Microsoft.AspNetCore.Authorization;

namespace Falc.Communications.Api.Endpoints;

public static class PatchUserEndpoint
{
    public static WebApplication RegisterPatchUserEndpoint(this WebApplication webApplication)
    {
        webApplication
            .MapPatch("users/{userId}", Handler)
            .RequireAuthorization()
            .WithTags(nameof(User))
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);
            
        return webApplication;
    }

    [Authorize(AuthenticationSchemes = HmacAuthenticationScheme.Name)]
    private static async Task<IResult> Handler(
        Guid userId,
        PatchUser.MarketingPreferencesDto marketingPreferences,
        PatchUser.ICommandHandler patchUserCommandHandler,
        CancellationToken cancellationToken)
    {
        var command = new PatchUser.Command(userId, marketingPreferences);
        await patchUserCommandHandler.ExecuteAsync(command, cancellationToken);
        return Results.NoContent();
    }
}
