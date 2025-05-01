using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;

namespace WebAPI.Controllers;

[Route("api/project")]
[ApiController]
[Authorize]
public class ProjectController : ControllerBase
{
    private readonly DbDiyProjectPlatformContext _dbContext;
    private readonly IMapper _mapper;
    
    public ProjectController(DbDiyProjectPlatformContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    // TODO: Check project status, show projects with ProjectStatusType.Approved
    // TODO: also maybe add another method to get pending projects (GetAllPendingProjects, or GetAllProjectsAndStatuses)

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

    [HttpDelete("delete")]
    public ActionResult DeleteProject(int id)
    {
        try
        { 
            var projectStatus = _dbContext.ProjectStatuses.FirstOrDefault(ps => ps.ProjectId == id);
            if (projectStatus == null)
                return NotFound("Project not found");
            
            var currentUserId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            
            projectStatus.StatusTypeId = (int)Enums.ProjectStatusType.Deleted;
            projectStatus.DateModified = DateTime.Now;
            projectStatus.ApproverId = currentUserId;
            _dbContext.SaveChanges();
            
            return Ok("Project has been deleted");
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}
