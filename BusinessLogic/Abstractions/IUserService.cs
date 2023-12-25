using BusinessLogic.Dtos;

namespace BusinessLogic.Abstractions;
public interface IUserService
{
    Task<UserDto> Register(RegisterUserDto userDto);
    Task<LoginResultDto> Login(LoginUserDto loginUserDto);
}
