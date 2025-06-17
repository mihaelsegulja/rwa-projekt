using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Core.Dtos;
using Core.Interfaces;
using Shared.Helpers;

namespace WebAPI.Controllers;

[Route("api/project")]
[ApiController]
[Authorize]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly IMapper _mapper;
    
    public ProjectController(IProjectService projectService, IMapper mapper)
    {
        _projectService = projectService;
        _mapper = mapper;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllProjects([FromQuery]ProjectFilterDto filter)
    {
        var userRole = ClaimsHelper.GetClaimValue(User, ClaimTypes.Role);
        var projects = await _projectService.GetAllProjectsAsync(userRole, filter);
        return Ok(projects);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProjectById(int id)
    {
        var project = await _projectService.GetProjectByIdAsync(id);
        return Ok(project);
    }

    [Authorize(Roles = nameof(Shared.Enums.UserRole.Admin))]
    [HttpGet("statuses")]
    public async Task<IActionResult> GetAllProjectStatuses(int page = 1, int pageSize = 10)
    {
        var projectStatuses = await _projectService.GetAllProjectStatusesAsync(page, pageSize);
        return Ok(projectStatuses);
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddProject(ProjectCreateDto project)
    {
        var currentUserId = ClaimsHelper.GetClaimValueAsInt(User, ClaimTypes.NameIdentifier);
        var result = await _projectService.AddProjectAsync(project, currentUserId);
        return Ok(result);
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateProject(ProjectUpdateDto projectUpdateDto)
    {
        var currentUserId = ClaimsHelper.GetClaimValueAsInt(User, ClaimTypes.NameIdentifier);
        var result = await _projectService.UpdateProjectAsync(projectUpdateDto, currentUserId);
        return Ok(result);
    }

    [Authorize(Roles = nameof(Shared.Enums.UserRole.Admin))]
    [HttpPut("update-status")]
    public async Task<IActionResult> UpdateProjectStatus(ProjectStatusDto projectStatus)
    {
        var result = await _projectService.UpdateProjectStatusAsync(projectStatus);
        return Ok(result);
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var currentUserId = ClaimsHelper.GetClaimValueAsInt(User, ClaimTypes.NameIdentifier);
        var result = await _projectService.DeleteProjectAsync(id, currentUserId);
        return Ok(result);
    }
}
