using Falc.IdentityProvider.Api.Proxy.Clients.IdentityProvider;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using System.Net.Http.Json;

namespace Falc.IdentityProvider.Api.Proxy.Endpoints;

public static class CreateUserEndpoint
{
    public sealed record Request(
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

    public sealed record Response(
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

    public static WebApplication RegisterCreateUserEndpoint(this WebApplication webApplication)
    {
        webApplication
            .MapPost("users", Handler)
            .AllowAnonymous()
            .WithTags("Users")
            .Produces(StatusCodes.Status201Created, typeof(Response));

        return webApplication;
    }

    [AllowAnonymous]
    private static async Task<IResult> Handler(
        Request request,
        IIdentityProviderHttpClient identityProviderHttpClient,
        CancellationToken cancellationToken)
    {
        var createUserRequest = new Clients.IdentityProvider.Models.CreateUserRequest(
            request.Id,
            request.UserName,
            request.Email,
            request.EmailConfirmed,
            request.PhoneNumber,
            request.PhoneNumberConfirmed,
            request.LockoutEnabled,
            request.TwoFactorEnabled,
            request.AccessFailedCount,
            request.LockoutEnd);

        using var identityProviderResponse = await identityProviderHttpClient.CreateUserAsync(createUserRequest, cancellationToken);

        if (identityProviderResponse.StatusCode != HttpStatusCode.Created)
        {
            return Results.StatusCode((int)identityProviderResponse.StatusCode);
        }

        var createUserResponse = await identityProviderResponse.Content.ReadFromJsonAsync<Clients.IdentityProvider.Models.CreateUserResponse>(cancellationToken);
        if (createUserResponse is null)
        {
            return Results.StatusCode(StatusCodes.Status502BadGateway);
        }

        var response = new Response(
            createUserResponse.Id,
            createUserResponse.UserName,
            createUserResponse.Email,
            createUserResponse.EmailConfirmed,
            createUserResponse.PhoneNumber,
            createUserResponse.PhoneNumberConfirmed,
            createUserResponse.LockoutEnabled,
            createUserResponse.TwoFactorEnabled,
            createUserResponse.AccessFailedCount,
            createUserResponse.LockoutEnd);

        return Results.Json(response, statusCode: StatusCodes.Status201Created);
    }
}
