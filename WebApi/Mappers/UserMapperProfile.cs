using AutoMapper;
using BusinessLogic.Dtos;
using WebApi.Models.Requests.User;
using WebApi.Models.Responses.User;

namespace WebApi.Mappers;

public class UserMapperProfile : Profile
{
    public UserMapperProfile() 
    {
        CreateMap<UserDto, UserResponse>()
            .ForMember(d => d.AvatarFileName, opt => opt.MapFrom(s => s.AvatarLocalFileName));
        CreateMap<UserTitleDto, UserTitleResponse>();
        CreateMap<UpdateUserRequest, UpdateUserDto>();
    }
}
