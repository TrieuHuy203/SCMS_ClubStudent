using System;
using System.Collections.Generic;

namespace SCMS.DomainEntities.Entities;

public partial class SurveyResult
{
    public int SurveyResultId { get; set; }

    public int SurveyId { get; set; }

    public int UserId { get; set; }

    public string? Answer { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Survey Survey { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
