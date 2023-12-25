using AutoMapper;
using BusinessLogic.Dtos;
using WebApi.Models.Requests.Post;
using WebApi.Models.Responses.Post;

namespace WebApi.Mappers;

public class PostMapperProfile : Profile
{
    public PostMapperProfile() 
    {
        CreateMap<CreatePostRequest, CreatePostDto>();
        CreateMap<UpdatePostRequest, UpdatePostDto>();

        CreateMap<PostItemRequest, PostItemDto>()
            .Include<TextPostItemRequest, TextPostItemDto>()
            .Include<ImagePostItemRequest, ImagePostItemDto>();
        CreateMap<TextPostItemRequest, TextPostItemDto>();
        CreateMap<ImagePostItemRequest, ImagePostItemDto>();

        CreateMap<PostDto, PostResponse>();
        CreateMap<PostItemDto, PostItemResponse>()
            .Include<TextPostItemDto, TextPostItemResponse>()
            .Include<ImagePostItemDto, ImagePostItemResponse>();
        CreateMap<TextPostItemDto, TextPostItemResponse>();
        CreateMap<ImagePostItemDto, ImagePostItemResponse>();

        CreateMap<PostTitleDto, PostTitleResponse>();
    }
}
