using AutoMapper;
using Core.Context;
using Core.Dtos;
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
                userRole == nameof(Shared.Enums.UserRole.Admin)
                    ? ps.StatusTypeId != (int)Shared.Enums.ProjectStatusType.Deleted
                    : ps.StatusTypeId == (int)Shared.Enums.ProjectStatusType.Approved))
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
       .Include(p => p.User)
       .Include(p => p.Topic)
       .Include(p => p.DifficultyLevel)
       .FirstOrDefaultAsync(p => p.Id == id);

        if (project == null) return null;

        var dto = new ProjectDetailDto
        {
            Project = _mapper.Map<ProjectDto>(project),
            Materials = project.ProjectMaterials.Select(pm => new MaterialDto
            {
                Id = pm.MaterialId,
                Name = pm.Material.Name
            }).ToList(),
            Images = project.ProjectImages.Select(pi => new ImageDto
            {
                ImageData = pi.Image.ImageData,
                Description = pi.Image.Description,
                IsMainImage = pi.IsMainImage
            }).ToList(),
            Username = project.User.Username,
            TopicName = project.Topic.Name,
            DifficultyLevelName = project.DifficultyLevel.Name
        };

        return dto;
    }

    public async Task<IEnumerable<ProjectStatusListDto>> GetAllProjectStatusesAsync(int page, int pageSize)
    {
        var statuses = await _dbContext.ProjectStatuses
            .Include(s => s.Project)
                .ThenInclude(p => p.User)
            .Include(s => s.Approver)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var result = statuses.Select(s => new ProjectStatusListDto
        {
            Id = s.Id,
            ProjectId = s.ProjectId,
            ProjectTitle = s.Project.Title,
            StatusTypeId = s.StatusTypeId,
            ApproverUsername = s.Approver != null ? s.Approver.Username : string.Empty,
            AuthorUsername = s.Project.User.Username,
            DateModified = s.DateModified
        });

        return result;
    }

    public async Task<string> AddProjectAsync(ProjectCreateDto projectCreateDto, int currentUserId)
    {
        var project = _mapper.Map<Project>(projectCreateDto.Project);
        project.DateCreated = DateTime.UtcNow;
        project.DateModified = DateTime.UtcNow;
        project.UserId = currentUserId;

        await _dbContext.Projects.AddAsync(project);
        await _dbContext.SaveChangesAsync();

        var materials = projectCreateDto.MaterialIds
            .Select(id => new ProjectMaterial
            {
                ProjectId = project.Id,
                MaterialId = id
            }).ToList();

        await _dbContext.ProjectMaterials.AddRangeAsync(materials);

        if (projectCreateDto.Images.Count > 0)
        {
            var projectImages = new List<ProjectImage>();
            foreach (var imageDto in projectCreateDto.Images)
            {
                var image = _mapper.Map<Image>(imageDto);
                image.DateAdded = DateTime.UtcNow;
                await _dbContext.Images.AddAsync(image);
                await _dbContext.SaveChangesAsync();

                projectImages.Add(new ProjectImage
                {
                    ProjectId = project.Id,
                    ImageId = image.Id,
                    IsMainImage = imageDto.IsMainImage
                });
            }

            await _dbContext.ProjectImages.AddRangeAsync(projectImages); 
        }

        var isAdmin = await _dbContext.Users
            .Where(u => u.Id == currentUserId)
            .Select(u => u.UserRoleId == (int)Shared.Enums.UserRole.Admin)
            .FirstOrDefaultAsync();

        var status = new ProjectStatus
        {
            ProjectId = project.Id,
            StatusTypeId = isAdmin
                ? (int)Shared.Enums.ProjectStatusType.Approved
                : (int)Shared.Enums.ProjectStatusType.Pending,
            ApproverId = isAdmin ? currentUserId : null,
            DateModified = DateTime.UtcNow
        };

        await _dbContext.ProjectStatuses.AddAsync(status);

        await _dbContext.SaveChangesAsync();

        return "Project added successfully";
    }


    public async Task<string?> UpdateProjectAsync(ProjectUpdateDto projectUpdateDto, int currentUserId)
    {
        var project = await _dbContext.Projects
        .Include(p => p.ProjectMaterials)
        .FirstOrDefaultAsync(p => p.Id == projectUpdateDto.Project.Id);

        if (project == null)
            return null;

        var canUpdate = await _dbContext.Users
            .Where(u => u.Id == currentUserId || u.UserRoleId == (int)Shared.Enums.UserRole.Admin)
            .FirstOrDefaultAsync();

        if (canUpdate == null)
            return "You do not have permission to update this project";

        // Update core project info
        _mapper.Map(projectUpdateDto.Project, project);
        project.DateModified = DateTime.UtcNow;

        // Replace materials
        var existingMaterialIds = project.ProjectMaterials.Select(pm => pm.MaterialId).ToList();
        var newMaterialIds = projectUpdateDto.MaterialIds;

        // Remove old materials
        var materialsToRemove = project.ProjectMaterials
            .Where(pm => !newMaterialIds.Contains(pm.MaterialId))
            .ToList();
        _dbContext.ProjectMaterials.RemoveRange(materialsToRemove);

        // Add new materials
        var materialsToAdd = newMaterialIds
            .Where(id => !existingMaterialIds.Contains(id))
            .Select(id => new ProjectMaterial
            {
                ProjectId = project.Id,
                MaterialId = id
            });

        await _dbContext.ProjectMaterials.AddRangeAsync(materialsToAdd);
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

        status.StatusTypeId = (int)Shared.Enums.ProjectStatusType.Deleted;
        status.DateModified = DateTime.UtcNow;
        status.ApproverId = currentUserId;
        await _dbContext.SaveChangesAsync();

        return "Project deleted";
    }
}
