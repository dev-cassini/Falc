namespace Falc.RmqFacade.Api.Services.Dtos;

public record PaginatedDto<T>(
    int PageNumber,
    int totalNumberOfPages,
    int totalNumberOfRecords,
    IReadOnlyList<T> Records);