using System;
using System.Collections.Generic;

namespace SCMS.DomainEntities.Entities;

public partial class Achievement
{
    public int AchievementId { get; set; }

    public int UserId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? Type { get; set; }

    public DateTime? ReceivedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
