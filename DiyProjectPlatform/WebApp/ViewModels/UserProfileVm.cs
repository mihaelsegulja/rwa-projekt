using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels;

public class UserProfileVm
{
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    public string Username { get; set; }
    [Required]
    public string Email { get; set; }
    public string? Phone { get; set; }
    public string? ProfilePicture { get; set; }
}
