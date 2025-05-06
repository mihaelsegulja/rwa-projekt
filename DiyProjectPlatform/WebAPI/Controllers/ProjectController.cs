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

    // TODO: Check project status, show projects with ProjectStatusType.Approved
    // TODO: also maybe add another method to get pending projects (GetAllPendingProjects, or GetAllProjectsAndAnyStatus)
    // or add filter by StatusType

    [HttpGet("all")]
    public ActionResult<IEnumerable<ProjectDto>> GetAllProjects(int page = 1, int pageSize = 10)
    {
        try
        {
            var projects = _dbContext.Projects
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(_mapper.Map<ProjectDto>(projects));
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

    [Authorize(Roles = "Admin")]
    [HttpPut("status")]
    public ActionResult UpdateProjectStatus([FromBody] ProjectStatusDto projectStatus)
    {
        try
        {
            var projStatus = _dbContext.ProjectStatuses.FirstOrDefault(ps => ps.Id == projectStatus.Id);
            if (projStatus == null)
                return NotFound("Project not found");
            
            _dbContext.ProjectStatuses.Update(_mapper.Map<ProjectStatus>(projStatus));
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
            
            return Ok("Project has been deleted");
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}
