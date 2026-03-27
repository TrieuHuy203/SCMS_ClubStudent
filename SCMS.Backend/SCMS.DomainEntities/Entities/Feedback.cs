using System;
using System.Collections.Generic;

namespace SCMS.DomainEntities.Entities;

public partial class Feedback
{
    public int FeedbackId { get; set; }

    public int UserId { get; set; }

    public string Content { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? ProcessedAt { get; set; }

    public int? ProcessedBy { get; set; }

    public virtual User? ProcessedByNavigation { get; set; }

    public virtual User User { get; set; } = null!;
}
