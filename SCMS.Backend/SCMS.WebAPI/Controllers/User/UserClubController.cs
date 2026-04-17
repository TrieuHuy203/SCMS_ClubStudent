using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.Interfaces.iService;

namespace SCMS.WebAPI.Controllers
{
	[ApiController]
	[Route("api/user/clubs")]
	public class UserClubController : ControllerBase
	{
		private readonly IClubService _clubService;

		public UserClubController(IClubService clubService)
		{
			_clubService = clubService;
		}

		// User gửi yêu cầu tạo câu lạc bộ (chờ admin duyệt)
		[HttpPost("register")]
		public async Task<IActionResult> RegisterClub([FromBody] ClubCreateRequest request)
		{
			if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			try
			{
				var result = await _clubService.SubmitClubRegistrationAsync(request, userId);
				return Ok(new
				{
					message = "Đã gửi yêu cầu tạo câu lạc bộ, vui lòng chờ admin duyệt.",
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
		}

		// User xem danh sách câu lạc bộ (phân trang)
		[HttpGet("list")]
		public async Task<IActionResult> GetList(
			[FromQuery] int page = 1,
			[FromQuery] int pageSize = 10)
		{
			var normalizedPage = page < 1 ? 1 : page;
			var normalizedPageSize = pageSize < 1 ? 10 : pageSize;

			var result = await _clubService.GetClubsPagedAsync(normalizedPage, normalizedPageSize);
			return Ok(result);
		}

		// User tìm kiếm câu lạc bộ (search + pagination)
		[HttpGet("search")]
		public async Task<IActionResult> Search([FromQuery] ClubSearchRequest request)
		{
			request ??= new ClubSearchRequest();

			// User chỉ nên thấy club đang hoạt động
			request.Status = "Active";
			if (request.Page < 1) request.Page = 1;
			if (request.PageSize < 1) request.PageSize = 10;

			var result = await _clubService.SearchClubsPagedAsync(request);
			return Ok(result);
		}

		// User xem chi tiết một câu lạc bộ
		[HttpGet("detail/{id}")]
		public async Task<IActionResult> GetDetail(int id)
		{
			var club = await _clubService.GetClubByIdAsync(id);
			if (club == null)
			{
				return NotFound(new { message = "Không tìm thấy câu lạc bộ." });
			}

			if (string.Equals(club.Status, "Deleted", StringComparison.OrdinalIgnoreCase) ||
				string.Equals(club.Status, "Inactive", StringComparison.OrdinalIgnoreCase))
			{
				return NotFound(new { message = "Không tìm thấy câu lạc bộ." });
			}

			return Ok(club);
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
