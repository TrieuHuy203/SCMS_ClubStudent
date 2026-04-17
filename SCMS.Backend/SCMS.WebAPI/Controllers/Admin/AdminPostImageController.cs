using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SCMS.Contracts.Interfaces.iService;
using SCMS.Contracts.DTOs.Requests;

namespace SCMS.WebAPI.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/post-images")]
    public class AdminPostImageController : ControllerBase
    {
        private readonly IPostImageService _postImageService;

        public AdminPostImageController(IPostImageService postImageService)
        {
            _postImageService = postImageService;
        }

        // GET: api/admin/post-images/list
        [HttpGet("list")]
        public async Task<IActionResult> GetAll([FromQuery] PostImageSearchRequest request)
        {
            var result = await _postImageService.GetAllAsync(request);
            return Ok(result);
        }

        // GET: api/admin/post-images/detail/{id}
        [HttpGet("detail/{id}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var result = await _postImageService.GetDetailAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // DELETE: api/admin/post-images/delete/{id}
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _postImageService.DeleteAsync(id);
            if (!success) return NotFound();
            return Ok(new { message = "Xoá ảnh thành công" });
        }
    
    // nhóm trưởng xem ảnh của tất cả bài post trong club
[HttpGet("by-club/{clubId}")]
public async Task<IActionResult> GetImagesByClub(int clubId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
{
    var result = await _postImageService.GetImagesByClubAsync(clubId, page, pageSize);
    return Ok(result);
} 
    }
}