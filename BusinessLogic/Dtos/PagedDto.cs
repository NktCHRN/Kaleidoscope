namespace BusinessLogic.Dtos;
public class PagedDto<TDto, TPaginationParametersDto>
{
    public IEnumerable<TDto> Data { get; set; } = default!;
    public TPaginationParametersDto PaginationParameters { get; set; } = default!;
}
