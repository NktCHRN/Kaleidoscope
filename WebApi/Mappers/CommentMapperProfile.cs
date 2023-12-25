using AutoMapper;
using BusinessLogic.Dtos;
using WebApi.Models.Requests.Comment;
using WebApi.Models.Responses.Comment;

namespace WebApi.Mappers;

public class CommentMapperProfile : Profile
{
    public CommentMapperProfile()
    {
        CreateMap<CommentDto, CommentResponse>();

        CreateMap<CreateCommentRequest, CreateCommentDto>();
        CreateMap<UpdateCommentRequest, UpdateCommentDto>();
    }
}
