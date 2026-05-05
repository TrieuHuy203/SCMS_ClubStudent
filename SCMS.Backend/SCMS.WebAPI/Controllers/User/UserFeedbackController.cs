using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.DTOs.Search;
using SCMS.Contracts.Interfaces.iService;
using SCMS.WebAPI.Attributes;
using SCMS.DomainEntities.Enums;

namespace SCMS.WebAPI.Controllers
{
	[ApiController]
	[Route("api/user/feedbacks")]
	public class UserFeedbackController : ControllerBase
	{
		private readonly IFeedbackService _feedbackService;

		public UserFeedbackController(IFeedbackService feedbackService)
		{
			_feedbackService = feedbackService;
		}

		// User tạo feedback mới
		[Permission(AppPermission.User_Feedback_Create)]
		[HttpPost("create")]
		public async Task<IActionResult> Create([FromBody] CreateFeedbackRequest request)
		{
			if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			try
			{
				var result = await _feedbackService.CreateAsync(request, userId);
				return Ok(new
				{
					message = "Tạo feedback thành công.",
					data = result
				});
			}
			catch (ArgumentException ex)
			{
				return BadRequest(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = ex.Message });
			}
		}

		// User xem danh sách feedback của chính mình
		[Permission(AppPermission.User_Feedback_View_List)]
		[HttpGet("my-feedbacks")]
		public async Task<IActionResult> GetMyFeedbacks([FromQuery] FeedbackSearchRequest request)
		{
			if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			request ??= new FeedbackSearchRequest();
			if (request.Page < 1) request.Page = 1;
			if (request.PageSize < 1) request.PageSize = 10;

			var result = await _feedbackService.GetMyFeedbacksAsync(userId, request);
			return Ok(result);
		}

		// User xem chi tiết một feedback của chính mình
		[Permission(AppPermission.User_Feedback_View_Detail)]
		[HttpGet("my-feedbacks/{id}")]
		public async Task<IActionResult> GetMyFeedbackDetail(int id)
		{
			if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			try
			{
				var result = await _feedbackService.GetMyFeedbackDetailAsync(id, userId);
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

		// User sửa feedback của chính mình
		[Permission(AppPermission.User_Feedback_Update)]
		[HttpPut("my-feedbacks/{id}")]
		public async Task<IActionResult> Update(int id, [FromBody] UpdateFeedbackRequest request)
		{
			if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			request.FeedbackId = id;

			try
			{
				var result = await _feedbackService.UpdateAsync(request, userId);
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

		// User xóa feedback của chính mình
		[Permission(AppPermission.User_Feedback_Delete)]
		[HttpDelete("my-feedbacks/{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			try
			{
				var result = await _feedbackService.DeleteAsync(id, userId);
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
			catch (InvalidOperationException ex)
			{
				return BadRequest(new { message = ex.Message });
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