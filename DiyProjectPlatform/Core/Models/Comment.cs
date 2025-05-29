using System;
using System.Collections.Generic;

namespace Core.Models;

public partial class Comment
{
    public int Id { get; set; }

    public DateTime DateCreated { get; set; }

    public string Content { get; set; } = null!;

    public int UserId { get; set; }

    public int ProjectId { get; set; }

    public int? ParentCommentId { get; set; }

    public virtual ICollection<Comment> InverseParentComment { get; set; } = new List<Comment>();

    public virtual Comment? ParentComment { get; set; }

    public virtual Project Project { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
