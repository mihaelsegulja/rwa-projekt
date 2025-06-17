namespace WebApp.ViewModels;

public class ProjectStatusListVm
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string ProjectTitle { get; set; } = string.Empty;
    public string AuthorUsername { get; set; } = string.Empty;
    public int StatusTypeId { get; set; }
    public string StatusTypeName => ((Shared.Enums.ProjectStatusType)StatusTypeId).ToString();
    public string ApproverUsername { get; set; } = string.Empty;
    public DateTime DateModified { get; set; }
    public int SelectedStatusTypeId { get; set; }
}
