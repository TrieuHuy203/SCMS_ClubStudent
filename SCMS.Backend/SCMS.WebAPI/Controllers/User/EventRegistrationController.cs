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
	public class EventRegistrationController : ControllerBase
	{
		private readonly IEventRegistrationService _eventRegistrationService;

		public EventRegistrationController(IEventRegistrationService eventRegistrationService)
		{
			_eventRegistrationService = eventRegistrationService;
		}

		// User đăng ký tham gia sự kiện
		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] EventRegisterRequest request)
		{
			if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			try
			{
				var result = await _eventRegistrationService.RegisterAsync(request, userId);
				return Ok(new
				{
					message = "Đăng ký sự kiện thành công.",
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
			catch (ArgumentNullException ex)
			{
				return BadRequest(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = ex.Message });
			}
		}

		// User xem danh sách sự kiện đã đăng ký của chính mình (search + paging)
		[HttpGet("my-registrations")]
		public async Task<IActionResult> GetMyRegistrations([FromQuery] EventRegistrationSearchRequest request)
		{
			if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			var result = await _eventRegistrationService.GetMyRegistrationsAsync(userId, request);
			return Ok(result);
		}

		// User xem chi tiết 1 đăng ký sự kiện thuộc về chính mình
		[HttpGet("my-registrations/{id}")] // id ở đây là EventRegistrationId
		public async Task<IActionResult> GetMyRegistrationDetail(int id)
		{
			if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			try
			{
				var result = await _eventRegistrationService.GetMyRegistrationDetailAsync(id, userId);
				return Ok(result);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(new { message = ex.Message });
			}
		}

		// User hủy đăng ký sự kiện của chính mình
		[HttpPost("my-registrations/{id}/cancel")] // id ở đây là EventRegistrationId
		public async Task<IActionResult> CancelMyRegistration(int id)
		{
			if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			try
			{
				var result = await _eventRegistrationService.CancelMyRegistrationAsync(id, userId);
				return Ok(new
				{
					message = "Hủy đăng ký sự kiện thành công.",
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

		// Nhóm trưởng xem danh sách thành viên tham gia sự kiện của câu lạc bộ mình tổ chức (search + paging)
		[HttpGet("my-clubs-events/{eventId}/registrations")]
		public async Task<IActionResult> GetMyClubEventRegistrations(
			int eventId,
			[FromQuery] EventRegistrationSearchRequest request)
		{
			if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			try
			{
				var result = await _eventRegistrationService.GetClubEventRegistrationsForMemberAsync(userId, eventId, request);
				return Ok(result);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(new { message = ex.Message });
			}
			catch (UnauthorizedAccessException)
			{
				return Forbid();
			}
		}

		// Nhóm trưởng export danh sách thành viên tham gia sự kiện của CLB mình tổ chức (CSV)
		[HttpGet("my-clubs-events/{eventId}/registrations/export")]
		public async Task<IActionResult> ExportMyClubEventRegistrations(int eventId)
		{
			if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			try
			{
				var fileBytes = await _eventRegistrationService.ExportClubEventRegistrationsForMemberAsync(userId, eventId);
				var fileName = $"event-{eventId}-registrations-{DateTime.Now:yyyyMMddHHmmss}.csv";
				return File(fileBytes, "text/csv", fileName);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(new { message = ex.Message });
			}
			catch (UnauthorizedAccessException)
			{
				return Forbid();
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = ex.Message });
			}
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
