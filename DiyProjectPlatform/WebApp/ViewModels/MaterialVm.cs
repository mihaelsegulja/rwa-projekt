using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels;

public class MaterialVm
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    public string Name { get; set; }
}
