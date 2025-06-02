namespace Core.Dtos;

public class CommentDto
{
    public int Id { get; set; }
    public DateTime DateCreated { get; set; }
    public string Content { get; set; }
    public int UserId { get; set; }
    public int ProjectId { get; set; }
    public int? ParentCommentId { get; set; }
}
