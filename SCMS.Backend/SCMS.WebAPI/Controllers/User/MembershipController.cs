using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.Interfaces.iService;

namespace SCMS.WebAPI.Controllers
{
    [ApiController]
    [Route("api/user/[controller]")]
    public class MembershipController : ControllerBase
    {
        private readonly IMembershipService _membershipService;

        public MembershipController(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }
        // API đăng ký tham gia CLB, user gửi yêu cầu đăng ký, trạng thái đơn sẽ là Pending
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateMembershipRequest request)
        {
            if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
            {
                return unauthorizedResult!;
            }

            try
            {
                var response = await _membershipService.RegisterAsync(request, userId);
                return Ok(new
                {
                    message = "Gửi đăng ký thành công!",
                    data = response
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


// api xem đơn đã đăng ký của chính mình, có filter trạng thái và phân trang
        [HttpGet("my-applications")]
            public async Task<IActionResult> GetMyApplications(
            [FromQuery] string? status,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
            {
                return unauthorizedResult!;
            }

            var result = await _membershipService.GetMyApplicationsAsync(userId, status, page, pageSize);
            return Ok(result);
        }

// API  xem chi tiết đơn đăng ký tham gia CLB của chính mình, chỉ cho phép xem chi tiết đơn của chính mình
        [HttpGet("my-applications/{id}")]
        public async Task<IActionResult> GetMyApplicationDetail(int id)
        {
            if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
            {
                return unauthorizedResult!;
            }

            try
            {
                var result = await _membershipService.GetMyApplicationDetailAsync(id, userId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
// api hủy đơn đăng ký của chính mình, chỉ cho phép khi đơn còn Pending
        [HttpPost("my-applications/{id}/cancel")] // id ở đây là MembershipId
        public async Task<IActionResult> CancelMyApplication(int id)
        {
            if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
            {
                return unauthorizedResult!;
            }

            try
            {
                var result = await _membershipService.CancelMyApplicationAsync(id, userId);
                return Ok(new
                {
                    message = "Hủy đơn đăng ký thành công.",
                    data = result
                });
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

        // API xem các CLB user đã tham gia (membership đã được duyệt)
        [HttpGet("my-clubs")]
        public async Task<IActionResult> GetMyJoinedClubs(
            [FromQuery] string? keyword,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
            {
                return unauthorizedResult!;
            }

            var result = await _membershipService.GetMyJoinedClubsAsync(userId, keyword, page, pageSize);
            return Ok(result);
        }

        // API xem danh sách thành viên của CLB mà user đã tham gia
        [HttpGet("my-clubs/{clubId}/members")]
        public async Task<IActionResult> GetMyClubMembers(
            int clubId,
            [FromQuery] string? keyword,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
            {
                return unauthorizedResult!;
            }

            try
            {
                var result = await _membershipService.GetMyClubMembersAsync(userId, clubId, keyword, page, pageSize);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        // API rời CLB đã tham gia
        [HttpPost("my-clubs/{clubId}/leave")]
        public async Task<IActionResult> LeaveClub(int clubId)
        {
            if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
            {
                return unauthorizedResult!;
            }

            try
            {
                var result = await _membershipService.LeaveClubAsync(clubId, userId);
                return Ok(new
                {
                    message = "Rời câu lạc bộ thành công.",
                    data = result
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        private bool TryGetCurrentUserId(out int userId, out IActionResult? unauthorizedResult)
        {
            unauthorizedResult = null;
            userId = 0;

            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out userId))
            {
                unauthorizedResult = Unauthorized(new { message = "Không tìm thấy UserId hợp lệ trong token!" });
                return false;
            }

            return true;
        }
       
        
    }
}