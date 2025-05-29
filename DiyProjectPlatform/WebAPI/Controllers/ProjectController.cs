using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Core.Dtos;
using Core.Models;
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
    public async Task<IActionResult> GetAllProjects(int page = 1, int pageSize = 10)
    {
        try
        {
            var userRole = ClaimsHelper.GetClaimValue(User, ClaimTypes.Role);
            var projects = await _projectService.GetAllProjectsAsync(userRole, page, pageSize);
            return Ok(projects);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProjectById(int id)
    {
        try
        {
            var project = await _projectService.GetProjectByIdAsync(id);
            return project == null ? NotFound("Project not found") : Ok(project);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [Authorize(Roles = nameof(Core.Enums.UserRole.Admin))]
    [HttpGet("statuses")]
    public async Task<IActionResult> GetAllProjectStatuses(int page = 1, int pageSize = 10)
    {
        try
        {
            var projectStatuses = await _projectService.GetAllProjectStatusesAsync(page, pageSize);
            return Ok(projectStatuses);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddProject(ProjectDetailDto projectDetail)
    {
        try
        {
            var result = _projectService.AddProjectAsync(projectDetail);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateProject(ProjectDto project)
    {
        try
        {
            var result = await _projectService.UpdateProjectAsync(project);
            return result == null ? NotFound() : Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [Authorize(Roles = nameof(Core.Enums.UserRole.Admin))]
    [HttpPut("update-status")]
    public async Task<IActionResult> UpdateProjectStatus(ProjectStatusDto projectStatus)
    {
        try
        {
            var result = await _projectService.UpdateProjectStatusAsync(projectStatus);
            return result == null ? NotFound() : Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        try
        { 
            var currentUserId = ClaimsHelper.GetClaimValueAsInt(User, ClaimTypes.NameIdentifier);
            var result = await _projectService.DeleteProjectAsync(id, currentUserId);
            return result == null ? NotFound() : Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}
