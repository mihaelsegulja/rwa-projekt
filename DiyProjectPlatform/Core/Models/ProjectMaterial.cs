namespace Core.Models;

public partial class ProjectMaterial
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public int MaterialId { get; set; }

    public virtual Material Material { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;
}
