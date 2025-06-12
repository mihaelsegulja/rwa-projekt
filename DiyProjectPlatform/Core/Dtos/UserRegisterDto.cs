using Shared.Constants;
using System.ComponentModel.DataAnnotations;

namespace Core.Dtos;

public class UserRegisterDto
{
    [Required(ErrorMessage = "Username is required")]
    [RegularExpression(RegexConstants.UsernamePattern,
        ErrorMessage = "Username must only contain lowercase/uppercase letters, digits, dashes and underscores.")]
    public string Username { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    [RegularExpression(RegexConstants.PasswordPattern,
        ErrorMessage = "Password must be at least 8 characters long, contain at least 1 uppercase letter, 1 lowercase letter, and 1 digit.")]
    public string Password { get; set; }
    
    [Required(ErrorMessage = "First name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "First name should be between 2 and 50 characters long")]
    public string FirstName { get; set; }
    
    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name should be between 2 and 50 characters long")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [RegularExpression(RegexConstants.EmailPattern,
        ErrorMessage = "Email is not valid.")]
    public string Email { get; set; }
    public string? Phone { get; set; }
}