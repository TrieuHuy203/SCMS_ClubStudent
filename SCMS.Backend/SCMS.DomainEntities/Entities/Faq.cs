using System;
using System.Collections.Generic;

namespace SCMS.DomainEntities.Entities;

public partial class Faq
{
    public int Faqid { get; set; }

    public string Question { get; set; } = null!;

    public string Answer { get; set; } = null!;

    public string Type { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public virtual User? CreatedByNavigation { get; set; }
}
