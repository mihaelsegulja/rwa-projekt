namespace Core.Models;

public partial class Image
{
    public int Id { get; set; }

    public string ImageData { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime DateAdded { get; set; }

    public virtual ICollection<ProjectImage> ProjectImages { get; set; } = new List<ProjectImage>();
}
