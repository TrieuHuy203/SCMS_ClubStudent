using Microsoft.AspNetCore.Http;

namespace SCMS.Contracts.DTOs.Requests
{
    public class UploadAvatarRequest
    {
        public IFormFile Avatar { get; set; }
    }
}