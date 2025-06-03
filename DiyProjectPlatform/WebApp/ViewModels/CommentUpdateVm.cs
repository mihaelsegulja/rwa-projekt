using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels;

public class CommentUpdateVm
{
    public int Id { get; set; }
    public int ProjectId { get; set; }

    [Required(ErrorMessage = "Content is required.")]
    public string Content { get; set; }
}

