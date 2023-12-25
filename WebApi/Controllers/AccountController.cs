using AutoMapper;
using BusinessLogic.Abstractions;
using BusinessLogic.Dtos;
using BusinessLogic.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models.Requests.Account;
using WebApi.Models.Requests.User;
using WebApi.Models.Responses.Account;
using WebApi.Models.Responses.Common;
using WebApi.Models.Responses.User;

namespace WebApi.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _userService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IMapper _mapper;

    public AccountController(IAccountService userService, IMapper mapper, IRefreshTokenService refreshTokenService)
    {
        _userService = userService;
        _mapper = mapper;
        _refreshTokenService = refreshTokenService;
    }

    [HttpPost("register")]
    [ProducesResponseType(201)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<IActionResult> Register([FromBody] RegisterAccountRequest request)
    {
        var _ = await _userService.Register(_mapper.Map<RegisterAccountDto>(request));

        return StatusCode(201);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(TokensResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<IActionResult> Login([FromBody] LoginAccountRequest request)
    {
        var result = await _userService.Login(_mapper.Map<LoginAccountDto>(request));

        if (!result.IsSuccessful)
        {
            return Unauthorized(new ErrorResponse { ErrorMessage = result.ErrorMessage!});
        }

        return Ok(_mapper.Map<TokensResponse>(result.Tokens));
    }

    [HttpPost("tokens/refresh")]
    [ProducesResponseType(typeof(TokensResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<IActionResult> RefreshTokens([FromBody] TokensRequest request)
    {
        var result = await _refreshTokenService.Refresh(_mapper.Map<TokensDto>(request));

        return Ok(_mapper.Map<TokensResponse>(result));
    }

    [Authorize]
    [HttpDelete("tokens/revoke")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequest request)
    {
        await _refreshTokenService.Revoke(User.GetId().GetValueOrDefault(), request.RefreshToken);

        return NoContent();
    }

    [Authorize]
    [HttpGet("details")]
    [ProducesResponseType(typeof(UserResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<IActionResult> GetDetails()
    {
        var result = await _userService.GetDetails(User.GetId().GetValueOrDefault());

        return Ok(_mapper.Map<UserResponse>(result));
    }

    [Authorize]
    [HttpPut("details")]
    [ProducesResponseType(typeof(UserResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<IActionResult> UpdateDetails([FromBody] UpdateUserRequest request)
    {
        var result = await _userService.UpdateDetails(User.GetId().GetValueOrDefault(), _mapper.Map<UpdateUserDto>(request));

        return Ok(_mapper.Map<UserResponse>(result));
    }
}
