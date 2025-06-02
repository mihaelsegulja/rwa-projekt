namespace Core.Dtos;

public class ProjectUpdateDto
{
    public ProjectDto Project { get; set; }
    public List<int> MaterialIds { get; set; } = new();
}
