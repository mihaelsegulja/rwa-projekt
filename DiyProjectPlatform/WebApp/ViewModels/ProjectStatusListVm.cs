using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.ViewModels;

public class ProjectStatusApprovalVm
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string ProjectTitle { get; set; } = "";
    public string AuthorUsername { get; set; } = "";
    public int StatusTypeId { get; set; }
    public string StatusTypeName => ((Shared.Enums.ProjectStatusType)StatusTypeId).ToString();
    public string ApproverUsername { get; set; } = "";
    public DateTime DateModified { get; set; }
    public List<SelectListItem> StatusTypeOptions { get; set; } = new();
    public int SelectedStatusTypeId { get; set; }
}
