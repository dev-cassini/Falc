using Microsoft.AspNetCore.Authorization;

namespace Falc.IdentityProvider.Api.Proxy.Endpoints;

public static class CreateUserEndpoint
{
    public static WebApplication RegisterCreateUserEndpoint(this WebApplication webApplication)
    {
        webApplication
            .MapPost("users", Handler)
            .AllowAnonymous()
            .WithTags("Users")
            .Produces(StatusCodes.Status200OK);

        return webApplication;
    }

    [AllowAnonymous]
    private static IResult Handler()
    {
        return Results.Ok();
    }
}
