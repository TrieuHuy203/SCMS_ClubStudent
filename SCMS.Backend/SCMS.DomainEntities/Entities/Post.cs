using System;
using System.Collections.Generic;
namespace SCMS.DomainEntities.Entities;

public partial class Post
{
    public int PostId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public int? ClubId { get; set; }

    public int? EventId { get; set; }

    public int UserId { get; set; }

    public string? PostType { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Club? Club { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual Event? Event { get; set; }

    public virtual ICollection<FileAttachment> FileAttachments { get; set; } = new List<FileAttachment>();

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    public virtual ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();

    public virtual ICollection<Share> Shares { get; set; } = new List<Share>();

    public virtual User User { get; set; } = null!;
}
