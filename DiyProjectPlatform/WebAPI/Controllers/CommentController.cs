using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Dtos;
using WebAPI.Models;

namespace WebAPI.Controllers;

[Route("api/comment")]
[ApiController]
[Authorize]
public class CommentController : ControllerBase
{
    private readonly DbDiyProjectPlatformContext _dbContext;
    private readonly IMapper _mapper;

    public CommentController(DbDiyProjectPlatformContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    [HttpGet("all/{projectId}")]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetAllCommentsForProject(int projectId, int page = 1, int pageSize = 10)
    {
        try
        {
            var comments = await _dbContext.Comments
                .Where(c => c.ProjectId == projectId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<CommentDto>>(comments));
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
            comment.DateCreated = DateTime.UtcNow;
            await _dbContext.Comments.AddAsync(_mapper.Map<Comment>(comment));
            await _dbContext.SaveChangesAsync();
            
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
           var existingComment = await _dbContext.Comments.FirstOrDefaultAsync(c => c.Id == comment.Id);
           if (existingComment == null)
               return NotFound("Comment not found");
           
           _mapper.Map(comment, existingComment);
           await _dbContext.SaveChangesAsync();
           
           return Ok("Comment updated");
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
            var existingComment = await _dbContext.Comments.FirstOrDefaultAsync(c => c.Id == id);
            if (existingComment == null)
                return NotFound("Comment not found");

            existingComment.Content = "deleted";
            await _dbContext.SaveChangesAsync();
            
            return Ok("Comment deleted");
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}
