using System;
using System.Collections.Generic;

namespace SCMS.DomainEntities.Entities;

public partial class History
{
    public int HistoryId { get; set; }

    public string TableName { get; set; } = null!;

    public int RecordId { get; set; }

    public string FieldName { get; set; } = null!;

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public int ChangedBy { get; set; }

    public DateTime ChangedAt { get; set; }

    public virtual User ChangedByNavigation { get; set; } = null!;
}
