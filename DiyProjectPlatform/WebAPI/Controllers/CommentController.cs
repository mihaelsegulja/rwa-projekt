using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public ActionResult GetAllCommentsForProject(int projectId, int page = 1, int pageSize = 10)
    {
        try
        {
            var comments = _dbContext.Comments
                .Where(c => c.ProjectId == projectId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            
            return Ok(comments);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost("add")]
    public ActionResult AddComment(CommentDto comment)
    {
        try
        {
            comment.DateCreated = DateTime.UtcNow;
            _dbContext.Comments.Add(_mapper.Map<Comment>(comment));
            _dbContext.SaveChanges();
            
            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}
