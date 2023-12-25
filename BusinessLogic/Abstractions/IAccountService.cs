using BusinessLogic.Dtos;

namespace BusinessLogic.Abstractions;
public interface IAccountService
{
    Task<UserDto> Register(RegisterAccountDto userDto);
    Task<LoginResultDto> Login(LoginAccountDto loginUserDto);
    Task<UserDto> GetDetails(Guid userId);
    Task<UserDto> UpdateDetails(Guid userId, UpdateUserDto userDto);
}
