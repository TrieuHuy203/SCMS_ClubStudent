using System;
using System.Collections.Generic;

namespace SCMS.DomainEntities.Entities;

public partial class ClubFavorite
{
    public int ClubFavoriteId { get; set; }

    public int UserId { get; set; }

    public int ClubId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Club Club { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
