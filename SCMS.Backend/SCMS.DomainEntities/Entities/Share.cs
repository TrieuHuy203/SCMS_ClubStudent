using System;
using System.Collections.Generic;

namespace SCMS.DomainEntities.Entities;

public partial class Share
{
    public int ShareId { get; set; }

    public int UserId { get; set; }

    public int PostId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Post Post { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
