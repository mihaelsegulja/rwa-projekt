namespace WebApp.ViewModels;

public class ProjectDetailVm
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string TopicName { get; set; } = string.Empty;
    public string DifficultyLevelName { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; }
    public DateTime DateModified { get; set; }
    public List<string> MaterialNames { get; set; } = new();
    public List<ImageVm> Images { get; set; } = new();

    public List<CommentVm> Comments { get; set; } = new();
}
