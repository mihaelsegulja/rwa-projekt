using System.ComponentModel.DataAnnotations;

namespace Core.Dtos;

public class ChangePasswordDto
{
    [Required(ErrorMessage = "Current Password is required")]
    public string CurrentPassword { get; set; }
    [Required(ErrorMessage = "New Password is required")]
    [StringLength(255, MinimumLength = 8, ErrorMessage = "New Password should be at least 8 characters long")]
    public string NewPassword { get; set; }
}
