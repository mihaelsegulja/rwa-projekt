using Core.Dtos;
using Shared.Results;

namespace Core.Interfaces;

public interface IProjectService
{
    Task<PagedResult<ProjectListDto>> GetAllProjectsAsync(string userRole, ProjectFilterDto filter);
    Task<ProjectDetailDto> GetProjectByIdAsync(int id);
    Task<PagedResult<ProjectStatusListDto>> GetAllProjectStatusesAsync(int page, int pageSize);
    Task<string> AddProjectAsync(ProjectCreateDto projectCreateDto, int currentUserId);
    Task<string> UpdateProjectAsync(ProjectUpdateDto projectUpdateDto, int currentUserId);
    Task<string> UpdateProjectStatusAsync(ProjectStatusDto projectStatusDto);
    Task<string> DeleteProjectAsync(int projectId, int currentUserId);
}
