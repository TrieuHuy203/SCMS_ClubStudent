using System;

namespace SCMS.DomainEntities.Entities
{
    public class PostImage
    {
        public int ImageId { get; set; }
        public int PostId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? Caption { get; set; }
        public int? SortOrder { get; set; }
        public DateTime UploadedAt { get; set; }

        // Navigation property (nullable để tránh cảnh báo)
        public virtual Post? Post { get; set; }
    }
}