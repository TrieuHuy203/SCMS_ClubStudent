using System;
using System.Collections.Generic;

namespace SCMS.DomainEntities.Entities;
public partial class Tag
{
    public int TagId { get; set; }

    public string TagName { get; set; } = null!;

    public string TagType { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<ClubTag> ClubTags { get; set; } = new List<ClubTag>();

    public virtual ICollection<EventTag> EventTags { get; set; } = new List<EventTag>();

    public virtual ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
}
