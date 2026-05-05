using System;
using System.Collections.Generic;

namespace SCMS.DomainEntities.Entities;
public partial class Event
{
    public int EventId { get; set; }

    public int ClubId { get; set; }

    public string EventName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime EventTime { get; set; }

    public DateTime? EndDateTime { get; set; }

    public string? Location { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? ParticipantCount { get; set; }

    public int? MaxParticipants { get; set; }

    public string? RejectReason { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public int? ApprovedBy { get; set; }

    public virtual Club Club { get; set; } = null!;
    public bool IsDeleted { get; set; } = false;

    public virtual ICollection<EventRegistration> EventRegistrations { get; set; } = new List<EventRegistration>();

    public virtual ICollection<EventTag> EventTags { get; set; } = new List<EventTag>();

    public virtual ICollection<FileAttachment> FileAttachments { get; set; } = new List<FileAttachment>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual ICollection<Survey> Surveys { get; set; } = new List<Survey>();
}
