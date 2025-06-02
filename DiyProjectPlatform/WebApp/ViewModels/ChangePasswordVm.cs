using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels;

public class ChangePasswordVm
{
    [Required]
    public string CurrentPassword { get; set; }
    [Required]
    [StringLength(255, MinimumLength = 8, ErrorMessage = "New Password should be at least 8 characters long")]
    public string NewPassword { get; set; }
    [Required]
    [Compare("NewPassword")]
    public string ConfirmPassword { get; set; }
}
