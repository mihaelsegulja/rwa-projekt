using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;

namespace WebAPI.Controllers;

[Route("api/project")]
[ApiController]
public class ProjectController : ControllerBase
{
    private readonly DbDiyProjectPlatformContext _dbContext;
    
    public ProjectController(DbDiyProjectPlatformContext dbContext)
    {
        _dbContext = dbContext;
    }

    // TODO: Check project status, show projects with ProjectStatusType.Approved
    // TODO: also maybe add another method to get pending projects (GetAllPendingProjects, or GetAllProjectsAndStatuses)

    [Authorize]
    [HttpGet("all")]
    public ActionResult<IEnumerable<Project>> GetAllProjects(int page = 1, int pageSize = 10)
    {
        try
        {
            var projects = _dbContext.Projects
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return Ok(projects);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [Authorize]
    [HttpGet("{id}")]
    public ActionResult<Project> GetProjectById(int id)
    {
        try
        {
            var project = _dbContext.Projects.FirstOrDefault(p => p.Id == id);
            if (project == null)
                return NotFound("Project not found");
            return Ok(project);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}
