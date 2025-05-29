using Core.Dtos;

namespace Core.Interfaces;

public interface IProjectService
{
    Task<IEnumerable<ProjectListDto>> GetAllProjectsAsync(string userRole, int page, int pageSize);
    Task<ProjectDetailDto?> GetProjectByIdAsync(int id);
    Task<IEnumerable<ProjectStatusDto>> GetAllProjectStatusesAsync(int page, int pageSize);
    Task<string> AddProjectAsync(ProjectDetailDto projectDetailDto);
    Task<string?> UpdateProjectAsync(ProjectDto projectDto);
    Task<string?> UpdateProjectStatusAsync(ProjectStatusDto projectStatusDto);
    Task<string?> DeleteProjectAsync(int projectId, int currentUserId);
}
