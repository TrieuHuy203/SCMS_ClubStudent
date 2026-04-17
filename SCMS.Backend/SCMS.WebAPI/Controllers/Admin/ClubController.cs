using Microsoft.AspNetCore.Mvc;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.DTOs.Responses;
using SCMS.Contracts.Interfaces.iService;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCMS.WebAPI.Controllers.Admin
{
    /// <summary>
    /// Controller quản lý Club (CRUD cho admin)
    /// </summary>
    [ApiController]
    [Route("api/admin/[controller]")]
    public class ClubController : ControllerBase
    {
        private readonly IClubService _clubService;

        // Inject service qua constructor
        public ClubController(IClubService clubService)
        {
            _clubService = clubService;
        }

        /// <summary>
        /// Lấy danh sách tất cả các club
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClubResponse>>> GetAllClubs()
        {
            var clubs = await _clubService.GetAllClubsAsync();
            return Ok(clubs);
        }

        /// <summary>
        /// Lấy thông tin club theo Id (xem chi tiết thông tin club)
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ClubResponse>> GetClubById(int id)
        {
            var club = await _clubService.GetClubByIdAsync(id);
            if (club == null) return NotFound();
            return Ok(club);
        }

        /// <summary>
        /// Tạo mới một club
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateClub([FromBody] ClubCreateRequest request)
        {
            if (!TryGetCurrentUserId(out var adminId, out var unauthorizedResult))
            {
                return unauthorizedResult!;
            }

            var created = await _clubService.CreateClubAsync(request, adminId);
            return CreatedAtAction(nameof(GetClubById), new { id = created.ClubId }, new {
                message = "Tạo club thành công",
                data = created
            });
        }

        /// <summary>
        /// Cập nhật thông tin club
        /// </summary>
      [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClub(int id, [FromBody] ClubUpdateRequest request)
        {
            if (id != request.ClubId) return BadRequest("Id không khớp với ClubId trong request");

            if (!TryGetCurrentUserId(out var adminId, out var unauthorizedResult))
            {
                return unauthorizedResult!;
            }

            await _clubService.UpdateClubAsync(request, adminId);
            return Ok(new { message = "Cập nhật club thành công" });
        }

        /// <summary>
        /// Xóa club theo Id
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClub(int id)
        {
            if (!TryGetCurrentUserId(out var adminId, out var unauthorizedResult))
            {
                return unauthorizedResult!;
            }

            await _clubService.DeleteClubAsync(id, adminId);
            return Ok(new { message = "Xóa mềm câu lạc bộ thành công" });
        }

        /// <summary>
        /// Bật/tắt hoạt động club
        /// </summary>
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> SetClubStatus(int id, [FromQuery] bool isDisabled)
        {
            try
            {
                if (!TryGetCurrentUserId(out var adminId, out var unauthorizedResult))
                {
                    return unauthorizedResult!;
                }

                var result = await _clubService.SetClubDisabledAsync(id, isDisabled, adminId);
                if (!result)
                    return NotFound(new { message = "Không tìm thấy câu lạc bộ" });

                return Ok(new
                {
                    message = isDisabled ? "Đã vô hiệu hóa câu lạc bộ" : "Đã mở lại hoạt động câu lạc bộ"
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Lấy club theo trang (pagination), trả về danh sách club và tổng số club (để client biết có bao nhiêu trang)
        [HttpGet("paged")]
            public async Task<IActionResult> GetClubsPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
            {
                var result = await _clubService.GetClubsPagedAsync(page, pageSize);
                return Ok(result); // Trả về PagedResult<ClubResponse>
            }
              /// <summary>
        /// Tìm kiếm & phân trang club (lọc nâng cao)
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> SearchClubs([FromQuery] ClubSearchRequest request)
        {
            var result = await _clubService.SearchClubsPagedAsync(request);
            return Ok(result); // Trả về PagedResult<ClubResponse>
        }

        /// <summary>
        /// Lấy danh sách club đang chờ duyệt
        /// </summary>
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingClubs([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _clubService.GetPendingClubsAsync(page, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Duyệt yêu cầu tạo club
        /// </summary>
        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveClub(int id)
        {
            try
            {
                if (!TryGetCurrentUserId(out var adminId, out var unauthorizedResult))
                {
                    return unauthorizedResult!;
                }

                var result = await _clubService.ApproveClubAsync(id, adminId);
                return Ok(new { message = "Duyệt câu lạc bộ thành công", data = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Từ chối yêu cầu tạo club
        /// </summary>
        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectClub(int id, [FromBody] RejectClubRequest request)
        {
            try
            {
                if (!TryGetCurrentUserId(out var adminId, out var unauthorizedResult))
                {
                    return unauthorizedResult!;
                }

                // Admin truyền lý do từ chối để user biết cần bổ sung/chỉnh sửa thông tin nào.
                var result = await _clubService.RejectClubAsync(id, adminId, request.RejectReason);
                return Ok(new { message = "Từ chối câu lạc bộ thành công", data = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private bool TryGetCurrentUserId(out int userId, out IActionResult? unauthorizedResult)
        {
            unauthorizedResult = null;
            userId = 0;

            var userIdClaim = User.FindFirst("UserId") ?? User.FindFirst("userId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out userId))
            {
                unauthorizedResult = Unauthorized(new { message = "Không tìm thấy UserId hợp lệ trong token!" });
                return false;
            }

            return true;
        }
    }
}