namespace Core.Models;

public partial class ProjectImage
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public int ImageId { get; set; }

    public bool IsMainImage { get; set; }

    public virtual Image Image { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;
}
