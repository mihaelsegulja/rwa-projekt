using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels;

public class TopicVm
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    public string Name { get; set; }
}
