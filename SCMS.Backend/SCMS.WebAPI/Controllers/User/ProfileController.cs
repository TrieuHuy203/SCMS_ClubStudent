using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.Interfaces.iService;
using System.IO;
using System.Threading.Tasks;
using SCMS.WebAPI.Attributes; // using directive for the custom PermissionAttribute
using SCMS.DomainEntities.Enums; // using directive for AppPermission enum

namespace SCMS.WebAPI.Controllers.User
{
	[ApiController]
	[Route("api/user/profile")]
	[Authorize]
	public class ProfileController : ControllerBase
	{
		private readonly IUserService _userService;
		private readonly IWebHostEnvironment _environment;

		public ProfileController(IUserService userService, IWebHostEnvironment environment)
		{
			_userService = userService;
			_environment = environment;
		}

		// Lấy hồ sơ cá nhân của user đang đăng nhập
		[Permission(AppPermission.Profile_View_My_Profile)]
		[HttpGet]
		public async Task<IActionResult> GetMyProfile()
		{
			if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			try
			{
				var result = await _userService.GetMyProfileAsync(userId);
				return Ok(result);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = ex.Message });
			}
		}

		// Upload avatar lên backend và lưu URL vào hồ sơ cá nhân
		[Permission(AppPermission.Profile_Upload_Avatar)]
		[HttpPost("avatar")]
		public async Task<IActionResult> UploadAvatar([FromForm] UploadAvatarRequest request)
		{
			if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			if (request.Avatar == null || request.Avatar.Length == 0)
			{
				return BadRequest(new { message = "Vui lòng chọn một file ảnh hợp lệ." });
			}

			var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
			var extension = Path.GetExtension(request.Avatar.FileName).ToLowerInvariant();
			if (!allowedExtensions.Contains(extension))
			{
				return BadRequest(new { message = "Chỉ hỗ trợ các định dạng ảnh: jpg, jpeg, png, gif, webp." });
			}

			// Tạo thư mục lưu ảnh nếu chưa tồn tại
			var webRootPath = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
			var avatarFolderPath = Path.Combine(webRootPath, "uploads", "avatars");
			Directory.CreateDirectory(avatarFolderPath);

			// Đặt tên file mới để tránh trùng và tránh ghi đè file cũ
			var fileName = $"{Guid.NewGuid():N}{extension}";
			var filePath = Path.Combine(avatarFolderPath, fileName);

			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await request.Avatar.CopyToAsync(stream);
			}

			// URL public để frontend hiển thị ảnh
			var avatarUrl = $"/uploads/avatars/{fileName}";

			try
			{
				// Lưu lại avatar cũ để xóa sau khi cập nhật thành công, tránh rác trong thư mục uploads
				var oldAvatarUrl = (await _userService.GetMyProfileAsync(userId)).AvatarUrl;

				// Lấy profile hiện tại rồi update lại cùng avatar mới
				var currentProfile = await _userService.GetMyProfileAsync(userId);
				var updateRequest = new UserProfileUpdateRequest
				{
					FullName = currentProfile.FullName ?? string.Empty,
					Email = currentProfile.Email ?? string.Empty,
					Phone = currentProfile.Phone,
					AvatarUrl = avatarUrl
				};

				var result = await _userService.UpdateMyProfileAsync(userId, updateRequest);

				// Xóa avatar cũ nếu nó là file local trong uploads/avatars
				DeleteOldLocalAvatar(oldAvatarUrl);

				return Ok(new
				{
					message = "Upload avatar thành công.",
					avatarUrl,
					data = result
				});
			}
			catch
			{
				// Nếu update DB lỗi, xóa file vừa upload để tránh file rác trên server
				if (System.IO.File.Exists(filePath))
				{
					System.IO.File.Delete(filePath);
				}

				throw;
			}
		}

		// Chỉ xóa file avatar cũ nếu nó nằm trong thư mục uploads local của backend
		private void DeleteOldLocalAvatar(string? oldAvatarUrl)
		{
			if (string.IsNullOrWhiteSpace(oldAvatarUrl))
			{
				return;
			}

			// Chỉ xử lý các path local dạng /uploads/avatars/...
			if (!oldAvatarUrl.StartsWith("/uploads/avatars/", StringComparison.OrdinalIgnoreCase))
			{
				return;
			}

			var oldFileName = Path.GetFileName(oldAvatarUrl);
			if (string.IsNullOrWhiteSpace(oldFileName))
			{
				return;
			}

			var webRootPath = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
			var oldFilePath = Path.Combine(webRootPath, "uploads", "avatars", oldFileName);

			if (System.IO.File.Exists(oldFilePath))
			{
				System.IO.File.Delete(oldFilePath);
			}
		}

		// Cập nhật hồ sơ cá nhân của user đang đăng nhập
		[Permission(AppPermission.Profile_Update_My_Profile)]
		[HttpPut]
		public async Task<IActionResult> UpdateMyProfile([FromBody] UserProfileUpdateRequest request)
		{
			if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			try
			{
				   // Nếu không muốn cập nhật email, giữ nguyên email cũ
				   if (request.Email == null)
				   {
					   var currentProfile = await _userService.GetMyProfileAsync(userId);
					   request.Email = currentProfile.Email;
				   }
				   var result = await _userService.UpdateMyProfileAsync(userId, request);
				   return Ok(new
				   {
					   message = "Cập nhật hồ sơ cá nhân thành công.",
					   data = result
				   });
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
			catch (Exception ex)
			{
				return StatusCode(500, new { message = ex.Message });
			}
		}

		// Đổi mật khẩu của user đang đăng nhập
		[Permission(AppPermission.Profile_Change_Password)]
		[HttpPost("change-password")]
		public async Task<IActionResult> ChangePassword([FromBody] UserChangePasswordRequest request)
		{
			if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			try
			{
				var message = await _userService.ChangeMyPasswordAsync(userId, request);
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
			catch (Exception ex)
			{
				return StatusCode(500, new { message = ex.Message });
			}
		}

		// Lấy UserId từ token để đảm bảo user chỉ sửa chính hồ sơ của mình
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
