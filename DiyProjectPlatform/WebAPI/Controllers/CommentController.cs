using AutoMapper;
using Core.Dtos;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        try
        {
            var comments = await _commentService.GetAllCommentsByProjectIdAsync(projectId, page, pageSize);
            return Ok(comments);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddComment(CommentDto comment)
    {
        try
        {
            await _commentService.AddCommentAsync(comment);
            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateComment(CommentDto comment)
    {
        try
        {
            int currentUserId = ClaimsHelper.GetClaimValueAsInt(User, ClaimTypes.NameIdentifier);
            var result = await _commentService.UpdateCommentAsync(comment, currentUserId);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteComment(int id)
    {
        try
        {
            var result = await _commentService.DeleteCommentAsync(id);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}
