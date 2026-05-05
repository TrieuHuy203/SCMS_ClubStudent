using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.Interfaces.iService;
using SCMS.DomainEntities.Enums; 
using SCMS.WebAPI.Attributes; // using directive for the custom PermissionAttribute
namespace SCMS.WebAPI.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/membership")]
    public class AdminMembershipController : ControllerBase
    {
        private readonly IAdminMembershipService _service;

        public AdminMembershipController(IAdminMembershipService service)
        {
            _service = service;
        }
// Endpoint lấy danh sách membership với filter và phân trang   
        [Permission(AppPermission.Admin_Membership_View_List)]
        [HttpGet("list")]
        public async Task<IActionResult> List([FromQuery] AdminMembershipListRequestDto filter)
        {
            var result = await _service.SearchAsync(filter);
            return Ok(result);
        }
       
        // Endpoint riêng cho admin xem thành viên của một CLB (chỉ lấy status Approved)
        [Permission(AppPermission.Admin_Membership_View_Club_Members)]
        [HttpGet("clubs/{clubId}/members")]
        public async Task<IActionResult> GetClubMembers(
            int clubId,
            [FromQuery] string? keyword,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var filter = new AdminMembershipListRequestDto
            {
                ClubId = clubId,
                UserName = keyword,
                Status = "Approved",
                Page = page,
                PageSize = pageSize
            };

            var result = await _service.SearchAsync(filter);
            return Ok(result);
        }

        [Permission(AppPermission.Admin_Membership_View_Detail)]
        [HttpGet("detail/{id}")]
        public async Task<IActionResult> Detail(int id)
        {
            var result = await _service.GetDetailAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

// Endpoint duyệt đơn đăng ký
        [Permission(AppPermission.Admin_Membership_Approve)]
        [HttpPost("approve")]
        public async Task<IActionResult> Approve([FromBody] int membershipId)
        {
            if (!TryGetCurrentUserId(out var adminId, out var unauthorizedResult))
            {
                return unauthorizedResult!;
            }

            await _service.ApproveAsync(membershipId, adminId);
            return Ok(new { message = "Duyệt đơn thành công" });
        }

        [Permission(AppPermission.Admin_Membership_Reject)]
        [HttpPost("reject")]
        public async Task<IActionResult> Reject([FromBody] int membershipId)
        {
            if (!TryGetCurrentUserId(out var adminId, out var unauthorizedResult))
            {
                return unauthorizedResult!;
            }

            await _service.RejectAsync(membershipId, adminId);
            return Ok(new { message = "Đã từ chối đơn đăng ký" });
        }

        // Kick thành viên đã được duyệt ra khỏi CLB
        [Permission(AppPermission.Admin_Membership_Kick)]
        [HttpPost("kick")]
        public async Task<IActionResult> Kick([FromBody] int membershipId)
        {
            if (!TryGetCurrentUserId(out var adminId, out var unauthorizedResult))
            {
                return unauthorizedResult!;
            }

            try
            {
                await _service.KickAsync(membershipId, adminId);
                return Ok(new { message = "Đã kick thành viên khỏi CLB" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Lấy tất cả đơn đăng ký tham gia CLB của một user (dành cho admin)
        /// </summary>
        [Permission(AppPermission.Admin_Membership_View_User_Applications)]
        [HttpGet("users/{userId}/applications")]
        public async Task<IActionResult> GetUserApplications(
            int userId,
            [FromQuery] string? keyword,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            // Tạo filter để lấy tất cả đơn đăng ký của user (có thể lọc theo keyword nếu cần)
            var filter = new AdminMembershipListRequestDto
            {
                UserId = userId,
                UserName = keyword,
                Page = page,
                PageSize = pageSize
                // Không set Status để lấy tất cả đơn (Pending, Approved, Rejected...)
            };

            var result = await _service.SearchAsync(filter);
            return Ok(result);
        }

        // Lấy UserId hiện tại từ token để ghi nhận actor trong audit log.
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

        // Thêm vào trong class AdminMembershipController

/// <summary>
/// Lấy danh sách đơn đăng ký chờ duyệt của một CLB (status = Pending)
/// </summary>
[Permission(AppPermission.Admin_Membership_View_Pending_Applications)]
[HttpGet("clubs/{clubId}/pending-applications")]
public async Task<IActionResult> GetPendingApplications(
    int clubId,
    [FromQuery] string? keyword,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
{
    // Nếu bạn muốn kiểm tra quyền nhóm trưởng/phó chủ nhiệm thì kiểm tra ở tầng service
    var filter = new AdminMembershipListRequestDto
    {
        ClubId = clubId,
        UserName = keyword,
        Status = "Pending",
        Page = page,
        PageSize = pageSize
    };

    var result = await _service.SearchAsync(filter);
    return Ok(result);
}
       /// <summary>
/// Lấy danh sách đơn đăng ký đã từ chối của một CLB (status = Rejected)
/// </summary>
[Permission(AppPermission.Admin_Membership_View_Rejected_Applications)]
[HttpGet("clubs/{clubId}/rejected-applications")]
public async Task<IActionResult> GetRejectedApplications(
    int clubId,
    [FromQuery] string? keyword,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
{
    var filter = new AdminMembershipListRequestDto
    {
        ClubId = clubId,
        UserName = keyword,
        Status = "Rejected",
        Page = page,
        PageSize = pageSize
    };

    var result = await _service.SearchAsync(filter);
    return Ok(result);
} 
        
    }
}
