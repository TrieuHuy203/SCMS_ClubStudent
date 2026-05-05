using System;

namespace SCMS.DomainEntities.Entities;

public partial class PostReport
{
    public int PostReportId { get; set; }

    public int PostId { get; set; }

    public int UserId { get; set; }

    public string Reason { get; set; } = null!;

    public string? Description { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Post Post { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
