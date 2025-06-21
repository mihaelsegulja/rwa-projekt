namespace WebApp.ViewModels;

public class ProjectListVm
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; }
    public string TopicName { get; set; } = string.Empty;
    public string DifficultyLevel { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public int? MainImageId { get; set; }
}
