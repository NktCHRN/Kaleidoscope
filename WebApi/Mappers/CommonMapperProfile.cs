using AutoMapper;
using BusinessLogic.Dtos;
using WebApi.Models.Common;
using WebApi.Models.Responses.Common;

namespace WebApi.Mappers;

public class CommonMapperProfile : Profile
{
    public CommonMapperProfile() 
    {
        CreateMap<PaginationParametersApiModel, PaginationParametersDto>();
        CreateMap<PaginationParametersDto, PaginationParametersApiModel>();

        CreateMap(typeof(PagedDto<,>), typeof(PagedResponse<,>));
    }
}
