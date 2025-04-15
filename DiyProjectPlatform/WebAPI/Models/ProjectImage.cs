using System;
using System.Collections.Generic;

namespace WebAPI.Models;

public partial class ProjectImage
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public string Image { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;
}
