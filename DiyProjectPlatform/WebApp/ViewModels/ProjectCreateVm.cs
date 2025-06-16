using Core.Dtos;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.ViewModels;

public class ProjectCreateVm
{
    public ProjectVm Project { get; set; } = new();
    public List<int> SelectedMaterialIds { get; set; } = new();
    public List<MaterialDto> AllMaterials { get; set; } = new();
    public List<SelectListItem> Topics { get; set; } = new();
    public List<SelectListItem> DifficultyLevels { get; set; } = new();
}
