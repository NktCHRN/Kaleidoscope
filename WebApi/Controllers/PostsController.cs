using AutoMapper;
using BusinessLogic.Abstractions;
using BusinessLogic.Constants;
using BusinessLogic.Dtos;
using BusinessLogic.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models.Requests.Post;
using WebApi.Models.Responses.Common;
using WebApi.Models.Responses.Post;

namespace WebApi.Controllers;
[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostService _postService;
    private readonly IMapper _mapper;

    public PostsController(IPostService postService, IMapper mapper)
    {
        _postService = postService;
        _mapper = mapper;
    }

    [Authorize(Roles = RolesConstants.Author)]
    [HttpPost("~/api/blogs/{blogId}/posts")]
    [ProducesResponseType(typeof(PostResponse), 201)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<IActionResult> Create(Guid blogId, [FromBody] CreatePostRequest request)
    {
        var result = await _postService.Create(User.GetId().GetValueOrDefault(), blogId, _mapper.Map<CreatePostDto>(request));
        return StatusCode(201, _mapper.Map<PostResponse>(result));
    }

    [Authorize(Roles = RolesConstants.Author)]
    [HttpPut("{postId}")]
    [ProducesResponseType(typeof(PostResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<IActionResult> Update(Guid postId, [FromBody] UpdatePostRequest request)
    {
        var result = await _postService.Update(User.GetId().GetValueOrDefault(), postId, _mapper.Map<UpdatePostDto>(request));
        return Ok(_mapper.Map<PostResponse>(result));
    }

    [Authorize(Roles = RolesConstants.Author)]
    [HttpDelete("{postId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<IActionResult> Delete(Guid postId)
    {
        await _postService.Delete(User.GetId().GetValueOrDefault(), postId);
        return NoContent();
    }
}
