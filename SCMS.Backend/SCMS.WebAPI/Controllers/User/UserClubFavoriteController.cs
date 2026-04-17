using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.Interfaces.iService;

namespace SCMS.WebAPI.Controllers
{
	[ApiController]
	[Route("api/user/club-favorites")]
	public class UserClubFavoriteController : ControllerBase
	{
		private readonly IClubFavoriteService _clubFavoriteService;

		public UserClubFavoriteController(IClubFavoriteService clubFavoriteService)
		{
			_clubFavoriteService = clubFavoriteService;
		}

		// User yêu thích một câu lạc bộ
		[HttpPost("{clubId}/favorite")]
		public async Task<IActionResult> AddFavorite(int clubId)
		{
			if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			try
			{
				var result = await _clubFavoriteService.AddFavoriteAsync(userId, clubId);
				return Ok(new
				{
					message = "Yêu thích câu lạc bộ thành công.",
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
			catch (Exception ex)
			{
				return StatusCode(500, new { message = ex.Message });
			}
		}

		// User hủy yêu thích một câu lạc bộ
		[HttpDelete("{clubId}/favorite")]
		public async Task<IActionResult> RemoveFavorite(int clubId)
		{
			if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			try
			{
				var result = await _clubFavoriteService.RemoveFavoriteAsync(userId, clubId);
				return Ok(new
				{
					message = "Hủy yêu thích câu lạc bộ thành công.",
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
			catch (Exception ex)
			{
				return StatusCode(500, new { message = ex.Message });
			}
		}

		// User xem danh sách câu lạc bộ đã yêu thích (search + phân trang)
		[HttpGet("my-favorites")]
		public async Task<IActionResult> GetMyFavorites([FromQuery] ClubFavoriteSearchRequest request)
		{
			if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			request ??= new ClubFavoriteSearchRequest();
			if (request.Page < 1) request.Page = 1;
			if (request.PageSize < 1) request.PageSize = 10;

			var result = await _clubFavoriteService.GetMyFavoritesAsync(userId, request);
			return Ok(result);
		}

		// Kiểm tra user đã yêu thích câu lạc bộ hay chưa  
		// CLb nào đó có thể hiển thị trạng thái yêu thích của user để UI hiển thị đúng (ví dụ: tô đỏ icon trái tim nếu đã yêu thích)
		[HttpGet("{clubId}/status")]
		public async Task<IActionResult> CheckFavoriteStatus(int clubId)
		{
			if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			var isFavorite = await _clubFavoriteService.IsFavoriteAsync(userId, clubId);
			return Ok(new
			{
				clubId,
				isFavorite
			});
		}

		// Helper method để lấy UserId từ token, trả về false nếu không có hoặc không hợp lệ
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
