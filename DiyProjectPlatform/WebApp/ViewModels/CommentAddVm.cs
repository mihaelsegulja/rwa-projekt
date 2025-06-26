using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels;

public class CommentAddVm
{
    [Display(Name = "Comment")]
    [Required(ErrorMessage = "Comment content is required.")]
    public string Content { get; set; } = string.Empty;

    [Required]
    public int ProjectId { get; set; }
}
