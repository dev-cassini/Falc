using Falc.RmqFacade.Api.Services;
using Falc.RmqFacade.Api.Services.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Falc.RmqFacade.Api.Endpoints;

public static class GetQueuesEndpoint
{
    public static WebApplication RegisterGetQueuesEndpoint(this WebApplication webApplication)
    {
        webApplication
            .MapGet("queues", Handler)
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK, typeof(PaginatedDto<QueueDto>))
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);
            
        return webApplication;
    }
    
    private static async Task<IResult> Handler(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromQuery] string name,
        IRmqManagementApi rmqManagementApi,
        CancellationToken cancellationToken)
    {
        var queues = await rmqManagementApi.GetQueuesAsync(page, pageSize,name, cancellationToken);
        return Results.Ok(queues);
    }
}