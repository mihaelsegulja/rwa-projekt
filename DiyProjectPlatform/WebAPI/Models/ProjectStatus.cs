using System;
using System.Collections.Generic;

namespace WebAPI.Models;

public partial class ProjectStatus
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public int StatusTypeId { get; set; }

    public DateTime DateModified { get; set; }

    public int? ApproverId { get; set; }

    public virtual User? Approver { get; set; }

    public virtual Project Project { get; set; } = null!;

    public virtual ProjectStatusType StatusType { get; set; } = null!;
}
