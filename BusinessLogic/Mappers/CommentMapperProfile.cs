using AutoMapper;
using BusinessLogic.Dtos;
using DataAccess.Entities;

namespace BusinessLogic.Mappers;
public class CommentMapperProfile : Profile
{
    public CommentMapperProfile()
    {
        CreateMap<Comment, CommentDto>()
            .ForMember(d => d.UserBlogTag, opt => opt.MapFrom(s => s.User!.Blog!.Tag));
    }
}
