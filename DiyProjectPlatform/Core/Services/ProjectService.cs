using AutoMapper;
using Core.Context;
using Core.Dtos;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Services;

public class ProjectService : IProjectService
{
    private readonly DbDiyProjectPlatformContext _dbContext;
    private readonly IMapper _mapper;

    public ProjectService(DbDiyProjectPlatformContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProjectListDto>> GetAllProjectsAsync(string userRole, int page, int pageSize)
    {
        var projects = await _dbContext.Projects
            .Where(p => p.ProjectStatuses.Any(ps =>
                userRole == nameof(Enums.UserRole.Admin)
                    ? ps.StatusTypeId != (int)Enums.ProjectStatusType.Deleted
                    : ps.StatusTypeId == (int)Enums.ProjectStatusType.Approved))
            .Include(p => p.User)
            .Include(p => p.Topic)
            .Include(p => p.DifficultyLevel)
            .Include(p => p.ProjectImages).ThenInclude(pi => pi.Image)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var result = projects.Select(p => new ProjectListDto
        {
            Id = p.Id,
            Title = p.Title,
            Description = p.Description,
            DateCreated = p.DateCreated,
            TopicName = p.Topic.Name,
            DifficultyLevel = p.DifficultyLevel.Name,
            Username = p.User.Username,
            MainImage = p.ProjectImages.FirstOrDefault(i => i.IsMainImage)?.Image.ImageData
        });

        return result;
    }

    public async Task<ProjectDetailDto?> GetProjectByIdAsync(int id)
    {
        var project = await _dbContext.Projects
            .Include(p => p.ProjectMaterials)
                .ThenInclude(pm => pm.Material)
            .Include(p => p.ProjectImages)
                .ThenInclude(pi => pi.Image)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project == null) return null;

        var dto = new ProjectDetailDto
        {
            Project = _mapper.Map<ProjectDto>(project),
            MaterialIds = project.ProjectMaterials.Select(pm => pm.MaterialId).ToList(),
            Images = project.ProjectImages.Select(pi => new ImageDto
            {
                ImageData = pi.Image.ImageData,
                IsMainImage = pi.IsMainImage
            }).ToList()
        };

        return dto;
    }

    public async Task<IEnumerable<ProjectStatusDto>> GetAllProjectStatusesAsync(int page, int pageSize)
    {
        var statuses = await _dbContext.ProjectStatuses
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ProjectStatusDto>>(statuses);
    }

    public async Task<string> AddProjectAsync(ProjectDetailDto projectDetailDto)
    {
        var project = _mapper.Map<Project>(projectDetailDto.Project);
        project.DateCreated = DateTime.UtcNow;

        await _dbContext.Projects.AddAsync(project);
        await _dbContext.SaveChangesAsync();

        var materials = projectDetailDto.MaterialIds.Select(id => new ProjectMaterial { ProjectId = project.Id, MaterialId = id });
        await _dbContext.ProjectMaterials.AddRangeAsync(materials);

        foreach (var imageDto in projectDetailDto.Images)
        {
            var image = _mapper.Map<Image>(imageDto);
            image.DateAdded = DateTime.UtcNow;
            await _dbContext.Images.AddAsync(image);
            await _dbContext.SaveChangesAsync();

            var pi = new ProjectImage { ProjectId = project.Id, ImageId = image.Id, IsMainImage = imageDto.IsMainImage };
            await _dbContext.ProjectImages.AddAsync(pi);
        }

        var status = new ProjectStatus
        {
            ProjectId = project.Id,
            StatusTypeId = (int)Enums.ProjectStatusType.Pending,
            DateModified = DateTime.UtcNow
        };
        await _dbContext.ProjectStatuses.AddAsync(status);
        await _dbContext.SaveChangesAsync();

        return "Project added";
    }

    public async Task<string?> UpdateProjectAsync(ProjectDto projectDto)
    {
        var project = await _dbContext.Projects.FirstOrDefaultAsync(p => p.Id == projectDto.Id);
        if (project == null) return null;

        _mapper.Map(projectDto, project);
        project.DateModified = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return "Project updated";
    }

    public async Task<string?> UpdateProjectStatusAsync(ProjectStatusDto projectStatusDto)
    {
        var status = await _dbContext.ProjectStatuses.FirstOrDefaultAsync(s => s.Id == projectStatusDto.Id);
        if (status == null) return null;

        _mapper.Map(projectStatusDto, status);
        status.DateModified = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return "Project status updated";
    }

    public async Task<string?> DeleteProjectAsync(int projectId, int currentUserId)
    {
        var status = await _dbContext.ProjectStatuses.FirstOrDefaultAsync(s => s.ProjectId == projectId);
        if (status == null) return null;

        status.StatusTypeId = (int)Enums.ProjectStatusType.Deleted;
        status.DateModified = DateTime.UtcNow;
        status.ApproverId = currentUserId;
        await _dbContext.SaveChangesAsync();

        return "Project deleted";
    }
}
