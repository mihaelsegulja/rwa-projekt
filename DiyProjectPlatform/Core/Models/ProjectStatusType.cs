using System;
using System.Collections.Generic;

namespace Core.Models;

public partial class ProjectStatusType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<ProjectStatus> ProjectStatuses { get; set; } = new List<ProjectStatus>();
}
