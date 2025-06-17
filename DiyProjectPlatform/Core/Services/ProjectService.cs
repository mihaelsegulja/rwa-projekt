using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core.Context;
using Core.Dtos;
using Core.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;
using Shared.Exceptions;
using Shared.Results;

namespace Core.Services;

public class ProjectService : IProjectService
{
    private readonly DbDiyProjectPlatformContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ILogService _logService;

    public ProjectService(DbDiyProjectPlatformContext dbContext, IMapper mapper, ILogService logService)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _logService = logService;
    }

    public async Task<PagedResult<ProjectListDto>> GetAllProjectsAsync(string userRole, ProjectFilterDto filter)
    {
        var query = _dbContext.Projects
            .AsNoTracking()
            .Where(p => p.ProjectStatuses.Any(ps =>
                userRole == nameof(Shared.Enums.UserRole.Admin)
                    ? ps.StatusTypeId != (int)Shared.Enums.ProjectStatusType.Deleted
                    : ps.StatusTypeId == (int)Shared.Enums.ProjectStatusType.Approved))
            .Include(p => p.User)
            .Include(p => p.Topic)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
            query = query.Where(p => p.Title.Contains(filter.Search));

        if (filter.TopicId.HasValue)
            query = query.Where(p => p.TopicId == filter.TopicId);

        if (filter.DifficultyLevelId.HasValue)
            query = query.Where(p => p.DifficultyLevelId == filter.DifficultyLevelId);

        var totalItems = await query.CountAsync();

        var items = await query
            .OrderByDescending(p => p.DateCreated)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(p => new ProjectListDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                DateCreated = p.DateCreated,
                TopicName = p.Topic.Name,
                DifficultyLevel = ((Shared.Enums.DifficultyLevel)p.DifficultyLevelId).ToString(),
                Username = p.User.Username,
                MainImageId = p.ProjectImages
                    .Where(i => i.IsMainImage)
                    .Select(i => i.ImageId)
                    .FirstOrDefault()
            }).ToListAsync();

        return new PagedResult<ProjectListDto>
        {
            Items = items,
            TotalItems = totalItems,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<ProjectDetailDto> GetProjectByIdAsync(int id)
    {
        var project = await _dbContext.Projects
            .Where(p => p.ProjectStatuses.Any(ps =>
                ps.StatusTypeId != (int)Shared.Enums.ProjectStatusType.Deleted))
           .Include(p => p.ProjectMaterials)
               .ThenInclude(pm => pm.Material)
           .Include(p => p.ProjectImages)
           .Include(p => p.User)
           .Include(p => p.Topic)
           .FirstOrDefaultAsync(p => p.Id == id) 
           ?? throw new NotFoundException($"Project {id} not found");

        var imageIds = project.ProjectImages.Select(pi => pi.ImageId).ToList();

        var imageDescriptions = await _dbContext.Images
            .Where(i => imageIds.Contains(i.Id))
            .Select(i => new { i.Id, i.Description })
            .ToListAsync();

        var dto = new ProjectDetailDto
        {
            Project = _mapper.Map<ProjectDto>(project),
            Materials = project.ProjectMaterials.Select(pm => new MaterialDto
            {
                Id = pm.MaterialId,
                Name = pm.Material.Name
            }).ToList(),
            Images = imageIds.Select(id => new ImageShortDto
            {
                Id = id,
                Description = imageDescriptions.FirstOrDefault(i => i.Id == id)?.Description ?? string.Empty
            }).ToList(),
            Username = project.User.Username,
            TopicName = project.Topic.Name,
            DifficultyLevelName = ((Shared.Enums.DifficultyLevel)project.DifficultyLevelId).ToString()
        };

        return dto;
    }

    public async Task<PagedResult<ProjectStatusListDto>> GetAllProjectStatusesAsync(int page, int pageSize)
    {
        var query = _dbContext.ProjectStatuses
            .Include(s => s.Project)
                .ThenInclude(p => p.User)
            .Include(s => s.Approver);

        var totalItems = await query.CountAsync();

        var items = await query
            .OrderByDescending(s => s.DateModified)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectTo<ProjectStatusListDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return new PagedResult<ProjectStatusListDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems
        };
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
        await _logService.AddLogAsync($"Project {project.Id} created", LogLevel.Info);

        return "Project added successfully";
    }


    public async Task<string> UpdateProjectAsync(ProjectUpdateDto projectUpdateDto, int currentUserId)
    {
        var project = await _dbContext.Projects
            .Include(p => p.ProjectMaterials)
            .FirstOrDefaultAsync(p => p.Id == projectUpdateDto.Project.Id) 
            ?? throw new NotFoundException($"Project {projectUpdateDto.Project.Id} not found");

        var currentUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == currentUserId)
            ?? throw new NotFoundException($"User {currentUserId} not found");

        var isAuthor = project.UserId == currentUserId;
        var isAdmin = currentUser.UserRoleId == (int)Shared.Enums.UserRole.Admin;

        if (!isAuthor && !isAdmin)
        {
            await _logService.AddLogAsync($"User {currentUserId} with no permission tried to update project {project.Id}", LogLevel.Warning);
            throw new UnauthorizedAccessException("You do not have permission to update this project.");
        }

        _mapper.Map(projectUpdateDto.Project, project);
        project.DateModified = DateTime.UtcNow;

        var existingMaterialIds = project.ProjectMaterials.Select(pm => pm.MaterialId).ToList();
        var newMaterialIds = projectUpdateDto.MaterialIds;

        var materialsToRemove = project.ProjectMaterials
            .Where(pm => !newMaterialIds.Contains(pm.MaterialId))
            .ToList();
        _dbContext.ProjectMaterials.RemoveRange(materialsToRemove);

        var materialsToAdd = newMaterialIds
            .Where(id => !existingMaterialIds.Contains(id))
            .Select(id => new ProjectMaterial
            {
                ProjectId = project.Id,
                MaterialId = id
            });

        await _dbContext.ProjectMaterials.AddRangeAsync(materialsToAdd);
        await _dbContext.SaveChangesAsync();
        await _logService.AddLogAsync($"Project {project.Id} updated", LogLevel.Info);

        return "Project updated";
    }

    public async Task<string> UpdateProjectStatusAsync(ProjectStatusDto projectStatusDto)
    {
        var status = await _dbContext.ProjectStatuses.FirstOrDefaultAsync(s => s.Id == projectStatusDto.Id) 
            ?? throw new NotFoundException($"Project status {projectStatusDto.Id} not found");

        _mapper.Map(projectStatusDto, status);
        status.DateModified = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
        await _logService.AddLogAsync($"User {status.ApproverId} updated project status {status.Id}", LogLevel.Info);

        return "Project status updated";
    }

    public async Task<string> DeleteProjectAsync(int projectId, int currentUserId)
    {
        var status = await _dbContext.ProjectStatuses.FirstOrDefaultAsync(s => s.ProjectId == projectId) 
            ?? throw new NotFoundException($"Project status for project {projectId} not found");

        status.StatusTypeId = (int)Shared.Enums.ProjectStatusType.Deleted;
        status.DateModified = DateTime.UtcNow;
        status.ApproverId = currentUserId;
        await _dbContext.SaveChangesAsync();
        await _logService.AddLogAsync($"User {currentUserId} deleted project {projectId}", LogLevel.Info);

        return "Project deleted";
    }
}
