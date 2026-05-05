using System;

namespace SCMS.Contracts.DTOs.Responses
{
    public class PostImageResponse
    {
        public int ImageId { get; set; }
        public int PostId { get; set; }
        public string ImageUrl { get; set; }
        public string? Caption { get; set; }
        public int? SortOrder { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}