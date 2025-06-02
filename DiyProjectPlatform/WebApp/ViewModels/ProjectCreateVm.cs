using Core.Dtos;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.ViewModels;

public class ProjectCreateVm
{
    public ProjectDto Project { get; set; } = new();
    public List<int> SelectedMaterialIds { get; set; } = new();
    public List<ImageDto> Images { get; set; } = new();

    // Dropdowns
    public List<SelectListItem> Topics { get; set; } = new();
    public List<SelectListItem> DifficultyLevels { get; set; } = new();

    // Checkboxes
    public List<MaterialDto> AllMaterials { get; set; } = new();
}
