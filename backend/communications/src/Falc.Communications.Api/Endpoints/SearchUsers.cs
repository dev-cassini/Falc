using Falc.Communications.Application.Queries;
using Falc.Communications.Domain.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;

namespace Falc.Communications.Api.Endpoints;

public static class SearchUsersEndpoint
{
    public record Request(string? EmailAddress = null, int PageNumber = 1, int PageSize = 25);
    
    public static WebApplication RegisterSearchUsersEndpoint(this WebApplication webApplication)
    {
        webApplication
            .MapPost("users/search", Handler)
            .RequireAuthorization(builder =>
            {
                builder
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .RequireClaim(ClaimTypes.Role, "admin");
            })
            .WithTags(nameof(User))
            .Produces(StatusCodes.Status200OK, typeof(SearchUsers.Result))
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);

        return webApplication;
    }

    private static async Task<IResult> Handler(
        Request request,
        SearchUsers.IQueryHandler searchUsersQueryHandler,
        CancellationToken cancellationToken)
    {
        var errors = new Dictionary<string, string[]>();

        if (request.PageNumber < 1)
        {
            errors[nameof(request.PageNumber)] = ["PageNumber must be greater than 0."];
        }

        if (request.PageSize is < 1 or > 100)
        {
            errors[nameof(request.PageSize)] = ["PageSize must be between 1 and 100."];
        }

        if (errors.Count > 0)
        {
            return Results.ValidationProblem(errors);
        }

        if (string.IsNullOrWhiteSpace(request.EmailAddress))
        {
            var emptyResult = new SearchUsers.Result(
                Array.Empty<SearchUsers.UserDto>(),
                request.PageNumber,
                request.PageSize,
                0);

            return Results.Ok(emptyResult);
        }

        var query = new SearchUsers.Request(
            request.EmailAddress,
            request.PageNumber,
            request.PageSize);

        var result = await searchUsersQueryHandler.ExecuteAsync(query, cancellationToken);

        return Results.Ok(result);
    }
}
