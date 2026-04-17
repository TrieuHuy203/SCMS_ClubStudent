using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.DTOs.Search;
using SCMS.Contracts.Interfaces.iService;

namespace SCMS.WebAPI.Controllers
{
	[ApiController]
	[Route("api/user/comments")]
	public class UserCommentController : ControllerBase
	{
		private readonly ICommentService _commentService;

		public UserCommentController(ICommentService commentService)
		{
			_commentService = commentService;
		}

		// User tạo bình luận mới
		[HttpPost("create")]
		public async Task<IActionResult> Create([FromBody] CreateCommentRequest request)
		{
			if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			try
			{
				var result = await _commentService.CreateAsync(request, userId);
				return Ok(new
				{
					message = "Tạo bình luận thành công.",
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
			catch (Exception ex)
			{
				return StatusCode(500, new { message = ex.Message });
			}
		}

		// User xem danh sách bình luận của chính mình (search + pagination)
		[HttpGet("my-comments")]
		public async Task<IActionResult> GetMyComments([FromQuery] CommentSearchRequest request)
		{
			if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			request ??= new CommentSearchRequest();
			if (request.Page < 1) request.Page = 1;
			if (request.PageSize < 1) request.PageSize = 10;

			var result = await _commentService.GetMyCommentsAsync(userId, request);
			return Ok(result);
		}

		// User xem chi tiết một bình luận của chính mình
		[HttpGet("my-comments/{id}")]
		public async Task<IActionResult> GetMyCommentDetail(int id)
		{
			if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			try
			{
				var result = await _commentService.GetMyCommentDetailAsync(id, userId);
				return Ok(new
				{
					message = "Lấy chi tiết bình luận thành công.",
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

		// User cập nhật bình luận của chính mình
		[HttpPut("my-comments/{id}")]
		public async Task<IActionResult> Update(int id, [FromBody] UpdateCommentRequest request)
		{
			if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			request.CommentId = id;

			try
			{
				var result = await _commentService.UpdateAsync(request, userId);
				return Ok(new
				{
					message = "Cập nhật bình luận thành công.",
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
			catch (Exception ex)
			{
				return StatusCode(500, new { message = ex.Message });
			}
		}

		// User xóa bình luận của chính mình
		[HttpDelete("my-comments/{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			try
			{
				var result = await _commentService.DeleteAsync(id, userId);
				return Ok(new
				{
					message = "Xóa bình luận thành công.",
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

		// User like bình luận
		[HttpPost("{id}/like")]
		public async Task<IActionResult> LikeComment(int id)
		{
			if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			try
			{
				var likeCount = await _commentService.LikeCommentAsync(id, userId);
				return Ok(new
				{
					message = "Like bình luận thành công.",
					commentId = id,
					likeCount
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

		// User bỏ like bình luận
		[HttpDelete("{id}/like")]
		public async Task<IActionResult> UnlikeComment(int id)
		{
			if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			try
			{
				var likeCount = await _commentService.UnlikeCommentAsync(id, userId);
				return Ok(new
				{
					message = "Bỏ like bình luận thành công.",
					commentId = id,
					likeCount
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
