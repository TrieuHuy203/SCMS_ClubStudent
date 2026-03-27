using System;
using System.Collections.Generic;

namespace SCMS.DomainEntities.Entities;

public partial class Survey
{
    public int SurveyId { get; set; }

    public int? ClubId { get; set; }

    public int? EventId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? SurveyType { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Club? Club { get; set; }

    public virtual Event? Event { get; set; }

    public virtual ICollection<FileAttachment> FileAttachments { get; set; } = new List<FileAttachment>();

    public virtual ICollection<SurveyResult> SurveyResults { get; set; } = new List<SurveyResult>();
}
