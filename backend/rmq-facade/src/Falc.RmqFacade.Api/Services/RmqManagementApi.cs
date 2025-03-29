using Falc.RmqFacade.Api.Services.Dtos;

namespace Falc.RmqFacade.Api.Services;

public class RmqManagementApi(HttpClient httpClient) : IRmqManagementApi
{
    public async Task<PaginatedDto<QueueDto>> GetQueuesAsync(
        int page, 
        int pageSize, 
        string name, 
        CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(
            $"api/queues?page={page}&page_size={pageSize}&name={name}&pagination=true", 
            cancellationToken);
        
        var paginationDto = await response.Content.ReadFromJsonAsync<PaginatedDto<QueueDto>>(cancellationToken);
        if (paginationDto is null)
        {
            throw new Exception(); //TODO: handle non-successful http response
        }
        
        return paginationDto;
    }
}