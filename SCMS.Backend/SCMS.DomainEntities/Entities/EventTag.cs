using System;
using System.Collections.Generic;

namespace SCMS.DomainEntities.Entities;

public partial class EventTag
{
    public int EventTagId { get; set; }

    public int EventId { get; set; }

    public int TagId { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual Tag Tag { get; set; } = null!;
}
