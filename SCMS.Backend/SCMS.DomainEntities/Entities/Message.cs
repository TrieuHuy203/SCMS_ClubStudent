using System;
using System.Collections.Generic;

namespace SCMS.DomainEntities.Entities;

public partial class Message
{
    public int MessageId { get; set; }

    public int SenderId { get; set; }

    public int? ReceiverId { get; set; }

    public int? GroupId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime SentAt { get; set; }

    public string Status { get; set; } = null!;

    public virtual User? Receiver { get; set; }

    public virtual User Sender { get; set; } = null!;
}
