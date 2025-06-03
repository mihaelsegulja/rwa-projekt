namespace WebApp.ViewModels;

public class CommentVm
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; }
    public string Username { get; set; } = string.Empty;
}
