using AutoMapper;
using BusinessLogic.Abstractions;
using BusinessLogic.Dtos;
using BusinessLogic.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models.Common;
using WebApi.Models.Requests.Comment;
using WebApi.Models.Responses.Comment;
using WebApi.Models.Responses.Common;

namespace WebApi.Controllers;
[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;
    private readonly IMapper _mapper;

    public CommentsController(ICommentService commentService, IMapper mapper)
    {
        _commentService = commentService;
        _mapper = mapper;
    }

    [Authorize]
    [HttpPost("~/api/posts/{postId}/comments")]
    [ProducesResponseType(typeof(CommentResponse), 201)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<IActionResult> Create(Guid postId, [FromBody] CreateCommentRequest request)
    {
        var result = await _commentService.Create(User.GetId().GetValueOrDefault(), postId, _mapper.Map<CreateCommentDto>(request));
        return StatusCode(201, _mapper.Map<CommentResponse>(result));
    }

    [HttpGet("~/api/posts/{postId}/comments")]
    [ProducesResponseType(typeof(PagedResponse<CommentResponse, PaginationParametersApiModel>), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<IActionResult> GetPagedByBlogId(Guid postId, [FromQuery] PaginationParametersApiModel parameters)
    {
        var result = await _commentService.GetPagedByPostId(postId, _mapper.Map<PaginationParametersDto>(parameters));

        return Ok(_mapper.Map<PagedResponse<CommentResponse, PaginationParametersApiModel>>(result));
    }

    [Authorize]
    [HttpPut("{commentId}")]
    [ProducesResponseType(typeof(CommentResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<IActionResult> Update(Guid commentId, [FromBody] UpdateCommentRequest request)
    {
        var result = await _commentService.Update(User.GetId().GetValueOrDefault(), commentId, _mapper.Map<UpdateCommentDto>(request));
        return Ok(_mapper.Map<CommentResponse>(result));
    }

    [Authorize]
    [HttpDelete("{commentId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<IActionResult> Delete(Guid commentId)
    {
        await _commentService.Delete(User.GetId().GetValueOrDefault(), commentId);
        return NoContent();
    }
}
