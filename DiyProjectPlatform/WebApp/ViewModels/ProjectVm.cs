using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels;

public class ProjectVm
{
    public int Id { get; set; }

    [Display(Name = "Title")]
    [Required(ErrorMessage = "Title is required.")]
    [MinLength(3, ErrorMessage = "Title must contain minimum 3 characters.")]
    public string Title { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateModified { get; set; }

    [Display(Name = "Description")]
    [Required(ErrorMessage = "Description is required.")]
    [MinLength(3, ErrorMessage = "Title must contain minimum 3 characters.")]
    public string Description { get; set; }

    [Display(Name = "Content")]
    [Required(ErrorMessage = "Content is required.")]
    [MinLength(20, ErrorMessage = "Content must contain minimum 20 characters.")]
    public string Content { get; set; }

    [Display(Name = "Topic")]
    [Required(ErrorMessage = "Topic is required.")]
    public int TopicId { get; set; }
    public int UserId { get; set; }

    [Display(Name = "Difficulty level")]
    [Required(ErrorMessage = "Difficulty level is required.")]
    public int DifficultyLevelId { get; set; }
}
