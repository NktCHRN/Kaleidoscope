using AutoMapper;
using BusinessLogic.Dtos;
using DataAccess.Entities;

namespace BusinessLogic.Mappers;
public class BlogMapperProfile : Profile
{
    public BlogMapperProfile()
    {
        CreateMap<Blog, BlogDto>();
    }
}
