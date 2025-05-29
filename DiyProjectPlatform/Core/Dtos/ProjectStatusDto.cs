namespace Core.Dtos;

public class ProjectStatusDto
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public int StatusTypeId { get; set; }
    public DateTime DateModified { get; set; }
    public int ApproverId { get; set; }
}