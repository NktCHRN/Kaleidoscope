using AutoMapper;
using BusinessLogic.Dtos;
using DataAccess.Entities;

namespace BusinessLogic.Mappers;
public class PostMapperProfile : Profile
{
    public PostMapperProfile() 
    {
        CreateMap<CreatePostDto, Post>();
        CreateMap<PostItemDto, PostItem>()
            .Include<ImagePostItemDto, ImagePostItem>()
            .Include<TextPostItemDto, TextPostItem>();

        CreateMap<Post, PostDto>()
            .ForMember(d => d.BlogTag, opt => opt.MapFrom(s => s.Blog.Tag));
        CreateMap<PostItem, PostItemDto>()
            .Include<ImagePostItem, ImagePostItemDto>()
            .Include<TextPostItem, TextPostItemDto>();
    }
}
