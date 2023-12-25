using BusinessLogic.Abstractions;
using BusinessLogic.Constants;
using BusinessLogic.Dtos;
using BusinessLogic.Exceptions;
using BusinessLogic.Options;
using DataAccess.Abstractions;
using DataAccess.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace BusinessLogic.Services;
public class UserService : IUserService
{
    private readonly IJwtTokenProvider _jwtTokenProvider;
    private readonly UserManager<User> _userManager;
    private readonly IValidator<RegisterUserDto> _validator;
    private readonly IRepository<RefreshToken> _refreshTokenRepository;
    private readonly IOptions<TokenProvidersOptions> _tokenProvidersOptions;

    public UserService(UserManager<User> userManager, IValidator<RegisterUserDto> validator, IJwtTokenProvider jwtTokenProvider, IRepository<RefreshToken> refreshTokenRepository, IOptions<TokenProvidersOptions> tokenProvidersOptions)
    {
        _userManager = userManager;
        _validator = validator;
        _jwtTokenProvider = jwtTokenProvider;
        _refreshTokenRepository = refreshTokenRepository;
        _tokenProvidersOptions = tokenProvidersOptions;
    }

    public async Task<LoginResultDto> Login(LoginUserDto loginUserDto)
    {
        var user = await _userManager.FindByNameAsync(loginUserDto.Email);
        if (!await _userManager.CheckPasswordAsync(user!, loginUserDto.Password))
        {
            return new LoginResultDto
            {
                IsSuccessful = false,
                ErrorMessage = "Wrong email or password"
            };
        }

        var refreshToken = _jwtTokenProvider.GenerateRefreshToken();
        await _refreshTokenRepository.AddAsync(new RefreshToken
        {
            Token = refreshToken,
            UserId = user!.Id,
            ExpiryTime = DateTimeOffset.UtcNow.AddDays(_tokenProvidersOptions.Value.RefreshTokenLifetimeInDays)
        });
        return new LoginResultDto()
        {
            IsSuccessful = true,
            Tokens = new TokensDto()
            {
                AccessToken = _jwtTokenProvider.GenerateAccessToken(await GetClaimsAsync(user!)),
                RefreshToken = refreshToken
            }
        };
    }

    private async Task<IEnumerable<Claim>> GetClaimsAsync(User user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Email!),
                    new Claim(ClaimTypes.Email, user.Email!)
                };
        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));
        return claims;
    }

    public async Task<UserDto> Register(RegisterUserDto userDto)
    {
        var validationResults = _validator.Validate(userDto);
        if (!validationResults.IsValid)
        {
            throw new EntityValidationFailedException(validationResults.Errors);
        }

        var user = new User
        {
            UserName = userDto.Email,
            Email = userDto.Email,
            Name = userDto.Name,
        };
        var userCreationResults = await _userManager.CreateAsync(user, userDto.Password);
        if (!userCreationResults.Succeeded)
        {
            throw new EntityValidationFailedException(userCreationResults.Errors);
        }

        await _userManager.AddToRoleAsync(user, RolesConstants.RegisteredViewer);

        // TODO: Send email confirmation.

        return new UserDto
        {
            Email = user.Email,
            Id = user.Id,
            Name = user.Name,
        };
    }
}
