using System;
using System.Collections.Generic;
namespace SCMS.DomainEntities.Entities;

public partial class ClubTag
{
    public int ClubTagId { get; set; }

    public int ClubId { get; set; }

    public int TagId { get; set; }

    public virtual Club Club { get; set; } = null!;

    public virtual Tag Tag { get; set; } = null!;
}
