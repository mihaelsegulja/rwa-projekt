using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public async Task<ActionResult<IEnumerable<ProjectListDto>>> GetAllProjects(int page = 1, int pageSize = 10)
    {
        try
        {
            var userRole = ClaimsHelper.GetClaimValue(User, ClaimTypes.Role);
            
            var projects = await _dbContext.Projects
                .Where(p => p.ProjectStatuses.Any(ps => 
                    userRole == "Admin" ? 
                        ps.StatusTypeId != (int)Enums.ProjectStatusType.Deleted : 
                        ps.StatusTypeId == (int)Enums.ProjectStatusType.Approved
                        ))
                .Include(p => p.User)
                .Include(p => p.Topic)
                .Include(p => p.DifficultyLevel)
                .Include(p => p.ProjectImages)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProjectListDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    DateCreated = p.DateCreated,
                    TopicName = p.Topic.Name,
                    DifficultyLevel = p.DifficultyLevel.Name,
                    Username = p.User.Username,
                    MainImage = p.ProjectImages
                    .Where(pi => pi.IsMainImage)
                    .Select(pi => pi.Image.ImageData)
                    .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(projects);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectDetailDto>> GetProjectById(int id)
    {
        try
        {
            var project = await _dbContext.Projects
                .Include(p => p.ProjectMaterials)
                .ThenInclude(pm => pm.Material)
                .Include(p => p.ProjectImages)
                .ThenInclude(pi => pi.Image)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
                return NotFound("Project not found");

            return Ok(_mapper.Map<ProjectDetailDto>(project));
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [Authorize(Roles = nameof(Enums.UserRole.Admin))]
    [HttpGet("statuses")]
    public async Task<ActionResult<IEnumerable<ProjectStatusDto>>> GetAllProjectStatuses(int page = 1, int pageSize = 10)
    {
        try
        {
            var projectStatuses = await _dbContext.ProjectStatuses
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<ProjectStatusDto>>(projectStatuses));
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
            var project = _mapper.Map<Project>(projectDetail);
            project.DateCreated = DateTime.UtcNow;

            await _dbContext.Projects.AddAsync(project);
            await _dbContext.SaveChangesAsync();

            foreach (var materialId in projectDetail.MaterialIds)
            {
                var projectMaterial = new ProjectMaterial
                {
                    ProjectId = project.Id,
                    MaterialId = materialId
                };
                await _dbContext.ProjectMaterials.AddAsync(projectMaterial);
            }

            foreach (var image in projectDetail.Images)
            {
                var newImage = _mapper.Map<Image>(image);
                newImage.DateAdded = DateTime.UtcNow;
                await _dbContext.Images.AddAsync(newImage);
                await _dbContext.SaveChangesAsync();

                var projectImage = new ProjectImage
                {
                    ProjectId = project.Id,
                    ImageId = newImage.Id,
                    IsMainImage = image.IsMainImage
                };
                await _dbContext.ProjectImages.AddAsync(projectImage);
            }

            var projectStatus = new ProjectStatus
            {
                ProjectId = project.Id,
                StatusTypeId = (int)Enums.ProjectStatusType.Pending,
                DateModified = DateTime.UtcNow
            };
            await _dbContext.ProjectStatuses.AddAsync(projectStatus);
            await _dbContext.SaveChangesAsync();

            return Ok("Project added");
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
            var existingProject = await _dbContext.Projects.FirstOrDefaultAsync(p => p.Id == project.Id);
            if (existingProject == null)
                return NotFound("Project not found");

            _mapper.Map(project, existingProject);
            await _dbContext.SaveChangesAsync();

            return Ok("Project updated");
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [Authorize(Roles = nameof(Enums.UserRole.Admin))]
    [HttpPut("update-status")]
    public async Task<IActionResult> UpdateProjectStatus(ProjectStatusDto projectStatus)
    {
        try
        {
            var projStatus = await _dbContext.ProjectStatuses.FirstOrDefaultAsync(ps => ps.Id == projectStatus.Id);
            if (projStatus == null)
                return NotFound("Project not found");
            
            _mapper.Map(projectStatus, projStatus);
            await _dbContext.SaveChangesAsync();
            
            return Ok("Project status updated");
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
            var projectStatus = await _dbContext.ProjectStatuses.FirstOrDefaultAsync(ps => ps.ProjectId == id);
            if (projectStatus == null)
                return NotFound("Project not found");
            
            var currentUserId = ClaimsHelper.GetClaimValueAsInt(User, ClaimTypes.NameIdentifier);
            
            projectStatus.StatusTypeId = (int)Enums.ProjectStatusType.Deleted;
            projectStatus.DateModified = DateTime.Now;
            projectStatus.ApproverId = currentUserId;
            await _dbContext.SaveChangesAsync();
            
            return Ok("Project deleted");
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}
