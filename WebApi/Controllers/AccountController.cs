using AutoMapper;
using BusinessLogic.Abstractions;
using BusinessLogic.Dtos;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models.Requests.Account;
using WebApi.Models.Responses.Account;
using WebApi.Models.Responses.Common;

namespace WebApi.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _userService;
    private readonly IMapper _mapper;

    public AccountController(IAccountService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
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
}
