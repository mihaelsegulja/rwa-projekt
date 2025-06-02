namespace Core.Dtos;

public class ProjectCreateDto
{
    public ProjectDto Project { get; set; }
    public List<int> MaterialIds { get; set; } = new();
    public List<ImageDto> Images { get; set; } = new();
}
