using AutoMapper;
using BusinessLogic.Abstractions;
using BusinessLogic.Dtos;
using BusinessLogic.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models.Requests.Blog;
using WebApi.Models.Responses.Blog;
using WebApi.Models.Responses.Common;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlogsController : ControllerBase
{
    private readonly IBlogService _blogService;
    private readonly IMapper _mapper;

    public BlogsController(IBlogService blogService, IMapper mapper)
    {
        _blogService = blogService;
        _mapper = mapper;
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(BlogResponse), 201)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<IActionResult> Create(CreateBlogRequest request)
    {
        var result = await _blogService.Create(User.GetId().GetValueOrDefault(), _mapper.Map<CreateBlogDto>(request));

        return StatusCode(201, _mapper.Map<BlogResponse>(result));
    }
}
