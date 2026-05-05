
using Microsoft.AspNetCore.Http;

namespace SCMS.Contracts.DTOs.Requests
{
    public class CreatePostImageRequest
    {
        public int PostId { get; set; }

        // Đường dẫn ảnh sau khi upload, Controller sẽ truyền vào
        public string? ImageUrl { get; set; }

        public string? Caption { get; set; }
        public int? SortOrder { get; set; }

        public required IFormFile ImageFile { get; set; }
    }
}