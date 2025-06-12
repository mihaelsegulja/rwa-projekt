using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.ViewModels;

public class ProjectStatusFilterVm
{
    public List<ProjectStatusListVm> Statuses { get; set; } = new();
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalItems { get; set; }
    public List<SelectListItem> StatusTypeOptions { get; set; } = new();
}
