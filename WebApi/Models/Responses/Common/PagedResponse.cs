namespace WebApi.Models.Responses.Common;

public class PagedResponse<TResponse, TPaginationParameters>
{
    public IEnumerable<TResponse> Data { get; set; } = default!;
    public TPaginationParameters PaginationParameters { get; set; } = default!;
}
