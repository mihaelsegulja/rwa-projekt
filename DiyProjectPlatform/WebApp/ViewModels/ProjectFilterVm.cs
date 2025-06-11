using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.ViewModels;

public class ProjectFilterVm
{
    public string? Search { get; set; }
    public int? TopicId { get; set; }
    public int? DifficultyLevelId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 9;
    public int TotalItems { get; set; }
    public List<SelectListItem> Topics { get; set; } = new();
    public List<SelectListItem> DifficultyLevels { get; set; } = new();
    public List<ProjectListVm> Projects { get; set; } = new();
}
