using AutoMapper;
using BusinessLogic.Dtos;
using WebApi.Models.Requests.Blog;
using WebApi.Models.Responses.Blog;

namespace WebApi.Mappers;

public class BlogMapperProfile : Profile
{
    public BlogMapperProfile() 
    {
        CreateMap<CreateBlogRequest, CreateBlogDto>();
        CreateMap<UpdateBlogRequest, UpdateBlogDto>();

        CreateMap<BlogDto, BlogResponse>();
    }
}
