namespace Core.Dtos;

public class ProjectDetailDto
{
    public ProjectDto Project { get; set; }
    public List<MaterialDto> Materials { get; set; } = new();
    public List<ImageShortDto> Images { get; set; } = new();
    public string Username { get; set; }
    public string TopicName { get; set; }
    public string DifficultyLevelName { get; set; }
}
