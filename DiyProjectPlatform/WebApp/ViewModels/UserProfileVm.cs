using Shared.Constants;
using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels;

public class UserProfileVm
{
    [Display(Name = "First Name")]
    [Required(ErrorMessage = "Firstname is required.")]
    public string FirstName { get; set; }

    [Display(Name = "Last Name")]
    [Required(ErrorMessage = "Lastname is required.")]
    public string LastName { get; set; }

    [Display(Name = "Username")]
    [Required(ErrorMessage = "Username is required.")]
    [RegularExpression(RegexConstants.UsernamePattern, 
        ErrorMessage = "Username must only contain lowercase/uppercase letters, digits, dashes and underscores.")]
    public string Username { get; set; }

    [Display(Name = "Email")]
    [Required(ErrorMessage = "Email is required.")]
    [RegularExpression(RegexConstants.EmailPattern, 
        ErrorMessage = "Email is not valid.")]
    public string Email { get; set; }

    [Display(Name = "Phone")]
    public string? Phone { get; set; }
    public string? ProfilePicture { get; set; }
}
