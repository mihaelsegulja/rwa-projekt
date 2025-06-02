namespace Core.Dtos;

public class ProjectStatusListDto
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string ProjectTitle { get; set; } = string.Empty;
    public string AuthorUsername { get; set; } = string.Empty;
    public int StatusTypeId { get; set; }
    public DateTime DateModified { get; set; }
    public string ApproverUsername { get; set; } = string.Empty;
}
