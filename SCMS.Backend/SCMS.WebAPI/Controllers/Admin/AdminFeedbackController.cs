using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.DTOs.Search;
using SCMS.Contracts.Interfaces.iService;

namespace SCMS.WebAPI.Controllers.Admin
{
	[ApiController]
	[Route("api/admin/feedbacks")]
	public class AdminFeedbackController : ControllerBase
	{
		private readonly IFeedbackService _feedbackService;

		public AdminFeedbackController(IFeedbackService feedbackService)
		{
			_feedbackService = feedbackService;
		}

		// Admin xem tất cả feedback (search + pagination)
		[HttpGet("list")]
		public async Task<IActionResult> GetAll([FromQuery] FeedbackSearchRequest request)
		{
			request ??= new FeedbackSearchRequest();
			if (request.Page < 1) request.Page = 1;
			if (request.PageSize < 1) request.PageSize = 10;

			var result = await _feedbackService.GetAllFeedbacksAsync(request);
			return Ok(result);
		}

		// Admin xem chi tiết một feedback
		[HttpGet("detail/{id}")]
		public async Task<IActionResult> GetDetail(int id)
		{
			try
			{
				var result = await _feedbackService.GetFeedbackDetailAsync(id);
				return Ok(new
				{
					message = "Lấy chi tiết feedback thành công.",
					data = result
				});
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

		// Admin đánh dấu feedback đã xem
		[HttpPost("{id}/mark-reviewed")]
		public async Task<IActionResult> MarkReviewed(int id, [FromBody] MarkReviewedRequest request)
		{
			if (!TryGetCurrentUserId(out var adminId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			try
			{
				var result = await _feedbackService.MarkReviewedAsync(id, adminId);
				return Ok(new
				{
					message = "Đánh dấu feedback đã xem thành công.",
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

		// Admin đánh dấu feedback đã giải quyết
		[HttpPost("{id}/mark-resolved")]
		public async Task<IActionResult> MarkResolved(int id, [FromBody] MarkResolvedRequest request)
		{
			if (!TryGetCurrentUserId(out var adminId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			try
			{
				var result = await _feedbackService.MarkResolvedAsync(id, adminId);
				return Ok(new
				{
					message = "Đánh dấu feedback đã giải quyết thành công.",
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

		// Admin sửa feedback (dùng để chuẩn hoá nộ dung , nếu cần lưu vào hồ sơ)
		[HttpPut("update/{id}")]
		public async Task<IActionResult> Update(int id, [FromBody] UpdateFeedbackRequest request)
		{
			if (!TryGetCurrentUserId(out var adminId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			request.FeedbackId = id;

			try
			{
				var result = await _feedbackService.AdminUpdateAsync(request, adminId);
				return Ok(new
				{
					message = "Cập nhật feedback thành công.",
					data = result
				});
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
			catch (Exception ex)
			{
				return StatusCode(500, new { message = ex.Message });
			}
		}

		// Admin xóa feedback
		[HttpDelete("delete/{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			if (!TryGetCurrentUserId(out var adminId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			try
			{
				var result = await _feedbackService.AdminDeleteAsync(id, adminId);
				return Ok(new
				{
					message = "Xóa feedback thành công.",
					data = result
				});
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

		// Helper method để lấy UserId từ token
		private bool TryGetCurrentUserId(out int userId, out IActionResult? unauthorizedResult)
		{
			unauthorizedResult = null;
			userId = 0;

			var userIdClaim = User.FindFirst("UserId");
			if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out userId))
			{
				unauthorizedResult = Unauthorized(new { message = "Không tìm thấy hoặc UserId không hợp lệ trong token." });
				return false;
			}

			return true;
	
	
	
	
		}
	}
}