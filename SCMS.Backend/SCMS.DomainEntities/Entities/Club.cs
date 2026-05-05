using System;
using System.Collections.Generic;

namespace SCMS.DomainEntities.Entities;

public partial class Club
{
    public int ClubId { get; set; }

    public string ClubName { get; set; } = null!;

    public string? Description { get; set; }

    public string? Field { get; set; }

    public string? School { get; set; }
    public int? CreatedByUserId { get; set; } // Id người tạo CLB

// Nếu muốn navigation property:
public virtual User CreatedByUser { get; set; }

    public int? MemberCount { get; set; }

    public string? Status { get; set; }

    public string? RejectReason { get; set; }

    public bool IsDisabled { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<ClubFavorite> ClubFavorites { get; set; } = new List<ClubFavorite>();

    public virtual ICollection<ClubTag> ClubTags { get; set; } = new List<ClubTag>();

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual ICollection<Membership> Memberships { get; set; } = new List<Membership>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual ICollection<Survey> Surveys { get; set; } = new List<Survey>();
}
