using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.Interfaces.iService;

namespace SCMS.WebAPI.Controllers.User
{
    [ApiController]
    [Route("api/user/posts/images")]
    public class UserPostImageController : ControllerBase
    {
        private readonly IPostImageService _postImageService;

        public UserPostImageController(IPostImageService postImageService)
        {
            _postImageService = postImageService;
        }

        // Upload ảnh cho bài post
        [HttpPost("upload")]
        [RequestSizeLimit(10_000_000)] // Giới hạn 10MB, tùy chỉnh nếu cần
        public async Task<IActionResult> Upload([FromForm] CreatePostImageRequest request)
        {
            if (request.ImageFile == null || request.ImageFile.Length == 0)
                return BadRequest(new { message = "Vui lòng chọn file ảnh!" });

            // Lưu file vào wwwroot/uploads/postImg
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "postImg");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}_{request.ImageFile.FileName}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.ImageFile.CopyToAsync(stream);
            }

            // Gán đường dẫn ảnh vào request
            request.ImageUrl = $"/uploads/postImg/{fileName}";

            var result = await _postImageService.UploadImageAsync(request);
            return Ok(new
            {
                message = "Upload ảnh thành công.",
                data = result
            });
        }

        // Lấy danh sách ảnh của 1 bài post
        [HttpGet("{postId}")]
        public async Task<IActionResult> GetImages(int postId)
        {
            var images = await _postImageService.GetImagesByPostIdAsync(postId);
            return Ok(images);
        }

        // Xóa ảnh (nếu cần)
        [HttpDelete("{imageId}")]
        public async Task<IActionResult> Delete(int imageId)
        {
            await _postImageService.DeleteImageAsync(imageId);
            return Ok(new { message = "Xóa ảnh thành công." });
        }
    }
}