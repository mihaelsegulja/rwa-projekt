namespace WebAPI.Dtos;

public class ProjectDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateModified { get; set; }
    public string Description { get; set; }
    public string Content { get; set; }
    public int TopicId { get; set; }
    public int UserId { get; set; }
    public int DifficultyLevelId { get; set; }
}
