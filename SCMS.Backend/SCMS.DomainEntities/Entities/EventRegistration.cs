using System;
using System.Collections.Generic;

namespace SCMS.DomainEntities.Entities;

public partial class EventRegistration
{
    public int EventRegistrationId { get; set; }

    public int UserId { get; set; }

    public int EventId { get; set; }

    public string? RegistrationStatus { get; set; }

    public DateTime? CheckInTime { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
