namespace WebAPI.Models;

public partial class User
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string PasswordSalt { get; set; } = null!;

    public string? Phone { get; set; }

    public string? ProfilePicture { get; set; }

    public string? SecurityToken { get; set; }

    public bool IsActive { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime? DateDeleted { get; set; }

    public int UserRoleId { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<ProjectStatus> ProjectStatuses { get; set; } = new List<ProjectStatus>();

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    public virtual UserRole UserRole { get; set; } = null!;
}
