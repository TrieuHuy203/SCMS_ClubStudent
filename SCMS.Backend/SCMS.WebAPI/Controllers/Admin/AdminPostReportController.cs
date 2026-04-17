using Microsoft.AspNetCore.Mvc;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.Interfaces.iService;
using System.Threading.Tasks;

namespace SCMS.WebAPI.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/post-reports")]
    public class AdminPostReportController : ControllerBase
    {
        private readonly IPostReportService _service;

        public AdminPostReportController(IPostReportService service)
        {
            _service = service;
        }

        // Lấy danh sách report (lọc, phân trang)
        [HttpGet("list")]
        public async Task<IActionResult> GetAll([FromQuery] PostReportSearchRequest request)
        {
            var (items, total) = await _service.SearchAsync(request);
            return Ok(new
            {
                message = "Lấy danh sách báo cáo thành công.",
                data = items,
                totalCount = total
            });
        }

        // Lấy chi tiết một report
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { message = "Không tìm thấy báo cáo." });

            return Ok(new
            {
                message = "Lấy chi tiết báo cáo thành công.",
                data = result
            });
        }

        // Đánh dấu report đã xem
        [HttpPost("{id}/mark-reviewed")]
        public async Task<IActionResult> MarkReviewed(int id)
        {
            // Có thể kiểm tra quyền admin ở đây nếu cần
            var ok = await _service.MarkReviewedAsync(id, 0); // Truyền adminId nếu cần
            if (!ok)
                return BadRequest(new { message = "Không thể đánh dấu đã xem." });

            return Ok(new { message = "Đánh dấu báo cáo đã xem thành công." });
        }

        // Đánh dấu report đã xử lý
        [HttpPost("{id}/mark-resolved")]
        public async Task<IActionResult> MarkResolved(int id)
        {
            // Có thể kiểm tra quyền admin ở đây nếu cần
            var ok = await _service.MarkResolvedAsync(id, 0); // Truyền adminId nếu cần
            if (!ok)
                return BadRequest(new { message = "Không thể đánh dấu đã xử lý." });

            return Ok(new { message = "Đánh dấu báo cáo đã xử lý thành công." });
        }
    }
}