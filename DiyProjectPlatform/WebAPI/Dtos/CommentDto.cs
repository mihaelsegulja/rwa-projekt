namespace WebAPI.Dtos;

public class CommentDto
{
    public DateTime DateCreated { get; set; }
    public string Content { get; set; } = null!;
    public int UserId { get; set; }
    public int ProjectId { get; set; }
    public int? ParentCommentId { get; set; }
}
