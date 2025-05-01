namespace WebAPI.Models;

public partial class Project
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public DateTime DateCreated { get; set; }

    public DateTime DateModified { get; set; }

    public string Description { get; set; } = null!;

    public string Content { get; set; } = null!;

    public int TopicId { get; set; }

    public int UserId { get; set; }

    public int DifficultyLevelId { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual DifficultyLevel DifficultyLevel { get; set; } = null!;

    public virtual ICollection<ProjectImage> ProjectImages { get; set; } = new List<ProjectImage>();

    public virtual ICollection<ProjectMaterial> ProjectMaterials { get; set; } = new List<ProjectMaterial>();

    public virtual ICollection<ProjectStatus> ProjectStatuses { get; set; } = new List<ProjectStatus>();

    public virtual Topic Topic { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
