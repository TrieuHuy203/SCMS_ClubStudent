using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using SCMS.Contracts.Interfaces.iService;
using Microsoft.AspNetCore.Authorization;
using SCMS.WebAPI.Attributes;
using SCMS.DomainEntities.Enums;

namespace SCMS.WebAPI.Controllers
{
	[ApiController]
	[Route("api/user/events")]
	[Authorize]
	public class UserEventController : ControllerBase
	{
		private readonly IEventService _eventService;

		public UserEventController(IEventService eventService)
		{
			_eventService = eventService;
		}

		// User xem danh sách sự kiện (có filter theo keyword, clubId và phân trang) [có thể bị trùng lặp]
		// api này là dành cho admin không phải user
		[Permission(AppPermission.User_Event_View_List)]
		[HttpGet("list")]

		public async Task<IActionResult> GetList(
			[FromQuery] string? keyword,
			[FromQuery] int? clubId,
			[FromQuery] int page = 1,
			[FromQuery] int pageSize = 10)
		{
			var result = await _eventService.GetEventListAsync(keyword, clubId, page, pageSize);
			return Ok(result);
		}

		// User xem sự kiện thuộc các CLB mình đã tham gia (membership Approved)
		[Permission(AppPermission.User_Event_View_My_Clubs_Events)]
		[HttpGet("my-clubs-events")]
		public async Task<IActionResult> GetMyClubsEvents(
			[FromQuery] string? keyword,
			[FromQuery] int page = 1,
			[FromQuery] int pageSize = 10)
		{
			if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			var result = await _eventService.GetMyClubEventsAsync(userId, keyword, page, pageSize);
			return Ok(result);
		}

		// User tìm kiếm sự kiện theo từ khóa
		[Permission(AppPermission.User_Event_Search)]
		[HttpGet("search")]
		public async Task<IActionResult> Search(
			[FromQuery] string keyword,
			[FromQuery] int? clubId,
			[FromQuery] int page = 1,
			[FromQuery] int pageSize = 10)
		{
			try
			{
				var result = await _eventService.SearchEventAsync(keyword, clubId, page, pageSize);
				return Ok(result);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(new { message = $"Tìm kiếm sự kiện thất bại: {ex.Message}" });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = $"Tìm kiếm sự kiện thất bại: {ex.Message}" });
			}
		}

		// User xem chi tiết một sự kiện
		[Permission(AppPermission.User_Event_View_Detail)]
		[HttpGet("detail/{id}")]
		public async Task<IActionResult> GetDetail(int id)
		{
			var result = await _eventService.GetEventDetailAsync(id);
			if (result == null)
			{
				return NotFound(new { message = "Không tìm thấy sự kiện." });
			}

			return Ok(result);
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
