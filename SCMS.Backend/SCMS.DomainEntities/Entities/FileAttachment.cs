using System;
using System.Collections.Generic;

namespace SCMS.DomainEntities.Entities;

public partial class FileAttachment
{
    public int FileAttachmentId { get; set; }

    public string FileName { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public string FileType { get; set; } = null!;

    public int? FileSize { get; set; }

    public int? PostId { get; set; }

    public int? EventId { get; set; }

    public int? SurveyId { get; set; }

    public int? UserId { get; set; }

    public DateTime UploadedAt { get; set; }

    public virtual Event? Event { get; set; }

    public virtual Post? Post { get; set; }

    public virtual Survey? Survey { get; set; }

    public virtual User? User { get; set; }
}
