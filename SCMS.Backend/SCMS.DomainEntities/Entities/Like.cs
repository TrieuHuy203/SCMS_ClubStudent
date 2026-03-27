using System;
using System.Collections.Generic;

namespace SCMS.DomainEntities.Entities;

public partial class Like
{
    public int LikeId { get; set; }

    public int UserId { get; set; }

    public int? PostId { get; set; }

    public int? CommentId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Comment? Comment { get; set; }

    public virtual Post? Post { get; set; }

    public virtual User User { get; set; } = null!;
}
