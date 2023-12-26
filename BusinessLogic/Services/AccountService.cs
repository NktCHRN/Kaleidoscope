using AutoMapper;
using BusinessLogic.Abstractions;
using BusinessLogic.Constants;
using BusinessLogic.Dtos;
using BusinessLogic.Exceptions;
using BusinessLogic.Options;
using DataAccess.Abstractions;
using DataAccess.Entities;
using DataAccess.Specifications;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace BusinessLogic.Services;
public class AccountService : IAccountService
{
    private readonly IJwtTokenProvider _jwtTokenProvider;
    private readonly UserManager<User> _userManager;
    private readonly IValidator<RegisterAccountDto> _registerValidator;
    private readonly IRepository<RefreshToken> _refreshTokenRepository;
    private readonly IOptions<TokenProvidersOptions> _tokenProvidersOptions;
    private readonly IRepository<User> _userRepository;
    private readonly IValidator<UpdateUserDto> _updateUserValidator;
    private readonly IBlobRepository _blobRepository;
    private readonly IMapper _mapper;
    private readonly TimeProvider _timeProvider;

    public AccountService(UserManager<User> userManager, IValidator<RegisterAccountDto> validator, IJwtTokenProvider jwtTokenProvider, IRepository<RefreshToken> refreshTokenRepository, IOptions<TokenProvidersOptions> tokenProvidersOptions, IRepository<User> userRepository, IValidator<UpdateUserDto> updateUserValidator, IMapper mapper, IBlobRepository blobRepository, TimeProvider timeProvider)
    {
        _userManager = userManager;
        _registerValidator = validator;
        _jwtTokenProvider = jwtTokenProvider;
        _refreshTokenRepository = refreshTokenRepository;
        _tokenProvidersOptions = tokenProvidersOptions;
        _userRepository = userRepository;
        _updateUserValidator = updateUserValidator;
        _mapper = mapper;
        _blobRepository = blobRepository;
        _timeProvider = timeProvider;
    }

    public async Task<LoginResultDto> Login(LoginAccountDto loginUserDto)
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
            ExpiryTime = _timeProvider.GetUtcNow().AddDays(_tokenProvidersOptions.Value.RefreshTokenLifetimeInDays)
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
                    new (ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new (ClaimTypes.Name, user.Email!),
                    new (ClaimTypes.Email, user.Email!)
                };
        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));
        return claims;
    }

    public async Task<UserDto> Register(RegisterAccountDto userDto)
    {
        var validationResults = _registerValidator.Validate(userDto);
        if (!validationResults.IsValid)
        {
            throw new EntityValidationFailedException(validationResults.Errors);
        }

        var user = new User
        {
            UserName = userDto.Email,
            Email = userDto.Email,
            Name = userDto.Name,
            CreatedAt = _timeProvider.GetUtcNow()
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

    public async Task<UserDto> GetDetails(Guid userId)
    {
        var user = await _userRepository.FirstOrDefaultAsync(new UserByIdNoTrackingSpec(userId)) 
            ?? throw new EntityNotFoundException("User was not found");

        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> UpdateDetails(Guid userId, UpdateUserDto userDto)
    {
        var validationResults = _updateUserValidator.Validate(userDto);
        if (!validationResults.IsValid)
        {
            throw new EntityValidationFailedException(validationResults.Errors);
        }

        var user = await _userRepository.FirstOrDefaultAsync(new UserByIdSpec(userId))
            ?? throw new EntityNotFoundException("User was not found");

        if (user.AvatarLocalFileName != userDto.AvatarLocalFileName 
            && !string.IsNullOrEmpty(userDto.AvatarLocalFileName)
            && !await _blobRepository.ExistsAsync(userDto.AvatarLocalFileName))
        {
            throw new EntityValidationFailedException($"Image with name {userDto.AvatarLocalFileName} was not found");
        }

        user.Name = userDto.Name;
        user.AvatarLocalFileName = userDto.AvatarLocalFileName;
        if (user.Blog is not null)
        {
            user.Blog.Name = userDto.Name;
            user.Blog.AvatarLocalFileName = userDto.AvatarLocalFileName;
        }

        await _userRepository.UpdateAsync(user);

        return _mapper.Map<UserDto>(user);
    }
}
