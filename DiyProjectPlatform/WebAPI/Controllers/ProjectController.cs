using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Dtos;
using WebAPI.Helpers;
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

    [HttpGet("all")]
    public ActionResult<IEnumerable<ProjectDto>> GetAllProjects(int page = 1, int pageSize = 10)
    {
        try
        {
            var userRole = ClaimsHelper.GetClaimValue(User, ClaimTypes.Role);
            
            var projects = _dbContext.Projects
                .Where(p => p.ProjectStatuses.Any(ps => 
                    userRole == "Admin" ? 
                        ps.StatusTypeId != (int)Enums.ProjectStatusType.Deleted : 
                        ps.StatusTypeId == (int)Enums.ProjectStatusType.Approved
                        ))
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(_mapper.Map<IEnumerable<ProjectDto>>(projects));
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet("{id}")]
    public ActionResult<ProjectDto> GetProjectById(int id)
    {
        try
        {
            var project = _dbContext.Projects.FirstOrDefault(p => p.Id == id);
            if (project == null)
                return NotFound("Project not found");
           
            return Ok(_mapper.Map<ProjectDto>(project));
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost("add")]
    public ActionResult AddProject([FromBody] ProjectDto project)
    {
        try
        {
            project.DateCreated = DateTime.UtcNow;

            var projectStatus = new ProjectStatus
            {
                ProjectId = project.Id,
                StatusTypeId = (int)Enums.ProjectStatusType.Pending,
                DateModified = DateTime.UtcNow
            };

            _dbContext.Projects.Add(_mapper.Map<Project>(project));
            _dbContext.ProjectStatuses.Add(projectStatus);
            _dbContext.SaveChanges();

            return Ok("Project added");
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpPut("update")]
    public IActionResult UpdateProject(int id, ProjectDto project)
    {
        try
        {
            var existingProject = _dbContext.Projects.FirstOrDefault(p => p.Id == id);
            if (existingProject == null)
                return NotFound("Project not found");

            _dbContext.Projects.Update(_mapper.Map<Project>(project));
            _dbContext.SaveChanges();

            return Ok("Project updated");
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("update-status")]
    public ActionResult UpdateProjectStatus(int id, ProjectStatusDto projectStatus)
    {
        try
        {
            var projStatus = _dbContext.ProjectStatuses.FirstOrDefault(ps => ps.Id == id);
            if (projStatus == null)
                return NotFound("Project not found");
            
            _dbContext.ProjectStatuses.Update(_mapper.Map<ProjectStatus>(projectStatus));
            _dbContext.SaveChanges();
            
            return Ok("Project status updated");
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
            
            var currentUserId = ClaimsHelper.GetClaimValueAsInt(User, ClaimTypes.NameIdentifier);
            
            projectStatus.StatusTypeId = (int)Enums.ProjectStatusType.Deleted;
            projectStatus.DateModified = DateTime.Now;
            projectStatus.ApproverId = currentUserId;
            _dbContext.SaveChanges();
            
            return Ok("Project deleted");
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}
