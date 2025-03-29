using Falc.RmqFacade.Api.Services.Dtos;

namespace Falc.RmqFacade.Api.Services;

public interface IRmqManagementApi
{
    Task<PaginatedDto<QueueDto>> GetQueuesAsync(
        int page,
        int pageSize,
        string name,
        CancellationToken cancellationToken); 
}