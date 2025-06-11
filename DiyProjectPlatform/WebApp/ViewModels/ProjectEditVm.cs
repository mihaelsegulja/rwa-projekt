using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.ViewModels;

public class ProjectEditVm
{
    public ProjectVm Project { get; set; } = new();
    public List<int> SelectedMaterialIds { get; set; } = new();
    public List<SelectListItem> AllMaterials { get; set; } = new();
    public List<SelectListItem> Topics { get; set; } = new();
    public List<SelectListItem> DifficultyLevels { get; set; } = new();
}

