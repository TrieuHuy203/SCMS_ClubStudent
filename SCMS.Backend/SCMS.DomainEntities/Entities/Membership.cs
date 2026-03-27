using System;
using System.Collections.Generic;
namespace SCMS.DomainEntities.Entities;
public partial class Membership
{
    public int MembershipId { get; set; }

    public int UserId { get; set; }

    public int ClubId { get; set; }

    public string? Role { get; set; }

    public string? Status { get; set; }

    public DateTime? JoinedAt { get; set; }

    public DateTime? LeftAt { get; set; }

    public virtual Club Club { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
