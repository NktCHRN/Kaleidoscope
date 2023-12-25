using AutoMapper;
using BusinessLogic.Dtos;
using DataAccess.Entities;

namespace BusinessLogic.Mappers;
public class UserMapperProfile : Profile
{
    public UserMapperProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<User, UserTitleDto>();
        CreateMap<UpdateUserDto, User>();
    }
}
