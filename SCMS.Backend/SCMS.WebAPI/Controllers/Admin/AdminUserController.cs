using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.DTOs.Responses;
using SCMS.Contracts.Interfaces.iService;

namespace SCMS.WebAPI.Controllers.Admin
{
    // Đặt route cho controller, ví dụ: api/admin/user
    [Route("api/admin/user")]
    [ApiController]
    public class AdminUserController : ControllerBase
    {
        private readonly IUserService _userService;

        // Inject IUserService qua constructor
        public AdminUserController(IUserService userService)
        {
            _userService = userService;
        }

		// Hàm lấy danh sách user có phân trang
		[HttpGet]
		public async Task<IActionResult> GetUsersPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
		{
			var result = await _userService.GetUsersPagedAsync(page, pageSize);
			return Ok(result); // Trả về PagedResult<UserDetailResponse>
		}

		// Tạo mới user (Admin)
        // Endpoint POST: api/admin/user
        [HttpPost] // Định nghĩa endpoint để tạo user mới
        public async Task<IActionResult> CreateUser([FromBody] UserCreateRequest request)
        {
            // Gọi service để tạo user mới
            var result = await _userService.CreateUserAsync(request);

            // Nếu có lỗi (Message khác "Tạo user thành công"), trả về BadRequest
            if (result.UserId == 0)
                return BadRequest(result);

            // Thành công, trả về 201 Created và thông tin user
            return CreatedAtAction(nameof(CreateUser), new { id = result.UserId }, result);
        }
		// Cập nhật thông tin user
		 // PUT: api/admin/users/{id}
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateRequest request)
		{
			// Lấy actor từ token để audit ghi đúng người thực hiện cập nhật.
			var userIdClaim = User.FindFirst("UserId") ?? User.FindFirst("userId");
			if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var currentUserId))
			{
				return Unauthorized("Không tìm thấy UserId hợp lệ trong token!");
			}

			var result = await _userService.UpdateUserAsync(id, currentUserId, request);
			if (!string.IsNullOrEmpty(result.Message) && result.Message != "Cập nhật user thành công")
			{
				// Trả về lỗi nếu có message lỗi
				return BadRequest(result.Message);
			}
			return Ok(result);
		}


		// xoá user
	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteUser(int id)
	{
		// Lấy id user hiện tại từ token (ví dụ dùng JWT, claim tên "userId")
		var userIdClaim = User.FindFirst("userId");
		int currentUserId = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;

		var result = await _userService.DeleteUserAsync(id, currentUserId);
		if (!result)
		{
			if (id == currentUserId)
				return BadRequest("Bạn không thể xoá chính mình!");
			return NotFound("User không tồn tại");
		}
		return Ok("Xoá user thành công (xoá mềm)");
	}


		// Vô hiệu hóa user
		[HttpPatch("{id}/status")]
		public async Task<IActionResult> SetUserStatus(int id, [FromQuery] bool isDisabled)
		{
			// Sửa lỗi: claim trong token là "UserId" (chữ U hoa), nếu dùng "userId" sẽ bị null
			var userIdClaim = User.FindFirst("UserId");
			if (userIdClaim == null)
			{
				// Nếu không tìm thấy claim, trả về lỗi xác thực
				return Unauthorized("Không tìm thấy thông tin user trong token!");
			}
			int currentUserId = int.Parse(userIdClaim.Value);
			var result = await _userService.SetUserDisabledAsync(id, isDisabled, currentUserId);
			if (!result)
			{
				if (id == currentUserId)
					return BadRequest("Bạn không thể tự vô hiệu hoá/mở khoá chính mình!");
				return NotFound("User không tồn tại");
			}
			return Ok(isDisabled ? "Đã vô hiệu hoá user" : "Đã mở khoá user");
		}  


		// Lấy chi tiết thông tin user theo Id
		[HttpGet("{id}")]
		public async Task<IActionResult> GetUserDetail(int id)
		{
			var user = await _userService.GetUserDetailAsync(id);
			if (user == null)
				return NotFound(new { message = "User không tồn tại" });
			return Ok(user);
		}
		
		//  tìm kiếm user
	[HttpGet("search")]
public async Task<IActionResult> SearchUsers([FromQuery] UserSearchRequest request)
{
    var result = await _userService.SearchUsersPagedAsync(request);
    return Ok(result); // Trả về PagedResult<UserDetailResponse>
}

		// Admin reset mật khẩu cho user
		[HttpPost("{id}/reset-password")]
		public async Task<IActionResult> AdminResetPassword(int id, [FromBody] AdminResetPasswordRequest request)
		{
			try
			{
				var userIdClaim = User.FindFirst("UserId") ?? User.FindFirst("userId");
				if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var adminId))
				{
					return Unauthorized(new { message = "Không tìm thấy UserId hợp lệ trong token!" });
				}

				var message = await _userService.AdminResetUserPasswordAsync(adminId, id, request);
				return Ok(new { message });
			}
			catch (ArgumentException ex)
			{
				return BadRequest(new { message = ex.Message });
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(new { message = ex.Message });
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(new { message = ex.Message });
			}
		}
		
   // Lấy danh sách avatar user
[HttpGet("avatars")]
public async Task<IActionResult> GetAllUserAvatars()
{
    var result = await _userService.GetAllUserAvatarsAsync();
    return Ok(result);
}

// Lấy chi tiết avatar user theo userId
[HttpGet("{userId}/avatar")]
public async Task<IActionResult> GetUserAvatar(int userId)
{
    var result = await _userService.GetUserAvatarAsync(userId);
    if (result == null) return NotFound();
    return Ok(result);
}

// Xóa avatar user theo userId
[HttpDelete("{userId}/avatar")]
public async Task<IActionResult> DeleteUserAvatar(int userId)
{
    var success = await _userService.DeleteUserAvatarAsync(userId);
    if (!success) return NotFound();
    return Ok(new { message = "Xóa avatar thành công" });
}
   
   
   
    }
}