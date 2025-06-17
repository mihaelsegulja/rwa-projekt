using AutoMapper;
using Core.Dtos;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Helpers;
using System.Security.Claims;

namespace WebAPI.Controllers;

[Route("api/comment")]
[ApiController]
[Authorize]
public class CommentController : ControllerBase
{
    private readonly ICommentService _commentService;
    private readonly IMapper _mapper;

    public CommentController(ICommentService commentService, IMapper mapper)
    {
        _commentService = commentService;
        _mapper = mapper;
    }

    [HttpGet("all/{projectId}")]
    public async Task<IActionResult> GetAllCommentsForProject(int projectId, int page = 1, int pageSize = 10)
    {
        var comments = await _commentService.GetAllCommentsByProjectIdAsync(projectId, page, pageSize);
        return Ok(comments);
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddComment(CommentDto comment)
    {
        await _commentService.AddCommentAsync(comment);
        return Ok();
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateComment(CommentUpdateDto comment)
    {
        int currentUserId = ClaimsHelper.GetClaimValueAsInt(User, ClaimTypes.NameIdentifier);
        var result = await _commentService.UpdateCommentAsync(comment, currentUserId);
        return Ok(result);
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteComment(int id)
    {
        var result = await _commentService.DeleteCommentAsync(id);
        return Ok(result);
    }
}
