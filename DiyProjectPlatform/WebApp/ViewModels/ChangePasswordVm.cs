using Shared.Constants;
using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels;

public class ChangePasswordVm
{
    [Required(ErrorMessage = "Old password is required.")]
    public string CurrentPassword { get; set; }

    [Required(ErrorMessage = "New password is required.")]
    [RegularExpression(RegexConstants.PasswordPattern, 
        ErrorMessage = "Password must be at least 8 characters long, contain at least 1 uppercase letter, 1 lowercase letter, and 1 digit.")]
    public string NewPassword { get; set; }

    [Required(ErrorMessage = "Confirm password is required.")]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; }
}
