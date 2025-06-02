using Core.Dtos;

namespace Core.Interfaces;

public interface IProjectService
{
    Task<IEnumerable<ProjectListDto>> GetAllProjectsAsync(string userRole, int page, int pageSize);
    Task<ProjectDetailDto?> GetProjectByIdAsync(int id);
    Task<IEnumerable<ProjectStatusListDto>> GetAllProjectStatusesAsync(int page, int pageSize);
    Task<string> AddProjectAsync(ProjectCreateDto projectCreateDto, int currentUserId);
    Task<string?> UpdateProjectAsync(ProjectUpdateDto projectUpdateDto, int currentUserId);
    Task<string?> UpdateProjectStatusAsync(ProjectStatusDto projectStatusDto);
    Task<string?> DeleteProjectAsync(int projectId, int currentUserId);
}
