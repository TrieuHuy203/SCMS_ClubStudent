using System;
using System.Collections.Generic;

namespace SCMS.DomainEntities.Entities;

public partial class AuthLog
{
    public int AuthLogId { get; set; }

    public int UserId { get; set; }

    public string ActionType { get; set; } = null!;

    public string? DeviceInfo { get; set; }

    public string? Ipaddress { get; set; }

    public DateTime? ActionTime { get; set; }

    public string? Status { get; set; }

    public virtual User User { get; set; } = null!;
}
