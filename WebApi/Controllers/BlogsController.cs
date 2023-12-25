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
    public async Task<IActionResult> Create([FromBody] CreateBlogRequest request)
    {
        var result = await _blogService.Create(User.GetId().GetValueOrDefault(), _mapper.Map<CreateBlogDto>(request));

        return StatusCode(201, _mapper.Map<BlogResponse>(result));
    }

    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(BlogResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<IActionResult> Update(Guid id, [FromBody]UpdateBlogRequest request)
    {
        var result = await _blogService.Update(id, _mapper.Map<UpdateBlogDto>(request));

        return Ok(_mapper.Map<BlogResponse>(result));
    }
}
