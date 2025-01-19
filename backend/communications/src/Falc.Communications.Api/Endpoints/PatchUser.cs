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
        PatchUser.Command command,
        MediatR.ISender sender,
        CancellationToken cancellationToken)
    {
        await sender.Send(command, cancellationToken);
        return Results.NoContent();
    }
}