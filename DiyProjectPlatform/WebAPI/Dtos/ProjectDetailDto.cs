namespace WebAPI.Dtos;

public class ProjectDetailDto
{
    public ProjectDto Project { get; set; }
    public List<int> MaterialIds { get; set; } = new();
    public List<ImageDto> Images { get; set; } = new();
}
