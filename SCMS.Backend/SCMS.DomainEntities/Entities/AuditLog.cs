using System;
using System.Collections.Generic;

namespace SCMS.DomainEntities.Entities;

public partial class AuditLog
{
    public int AuditLogId { get; set; }

    public int UserId { get; set; }

    public string TableName { get; set; } = null!;

    public int RecordId { get; set; }

    public string ActionType { get; set; } = null!;

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public DateTime? ActionTime { get; set; }

    public virtual User User { get; set; } = null!;
}
