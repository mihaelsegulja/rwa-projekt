namespace Core.Dtos;

public class ProjectFilterDto
{
    public string? Search { get; set; }
    public int? TopicId { get; set; }
    public int? DifficultyLevelId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
