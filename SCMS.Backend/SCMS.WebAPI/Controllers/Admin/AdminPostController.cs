
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.Interfaces.iService;

namespace SCMS.WebAPI.Controllers.Admin
{
	[ApiController]
	[Route("api/admin/posts")]
	public class AdminPostController : ControllerBase
	{
		private readonly IPostService _postService;

		public AdminPostController(IPostService postService)
		{
			_postService = postService;
		}

		// Admin xem tất cả bài viết (search + pagination)
		[HttpGet("list")]
		public async Task<IActionResult> GetAll([FromQuery] PostSearchRequest request)
		{
			request ??= new PostSearchRequest();
			if (request.Page < 1) request.Page = 1;
			if (request.PageSize < 1) request.PageSize = 10;

			var result = await _postService.GetAllPostsAsync(request);
			return Ok(result);
		}

		// xem lisr post -pending của riêng CLB
		
		

		// Admin xem chi tiết một bài viết
		[HttpGet("detail/{id}")]
		public async Task<IActionResult> GetDetail(int id)
		{
			try
			{
				var result = await _postService.GetPostDetailAsync(id);
				return Ok(result);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(new { message = ex.Message });
			}
		}

		// Admin tạo bài viết mới
		[HttpPost("create")]
		public async Task<IActionResult> Create([FromBody] CreatePostRequest request)
		{
			if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			try
			{
				var result = await _postService.AdminCreateAsync(request, userId);
				return Ok(new
				{
					message = "Tạo bài viết thành công.",
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

		// Admin cập nhật bài viết
		[HttpPut("update/{id}")]
		public async Task<IActionResult> Update(int id, [FromBody] UpdatePostRequest request)
		{
			if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			request.PostId = id;

			try
			{
				var result = await _postService.AdminUpdateAsync(request, userId);
				return Ok(new
				{
					message = "Cập nhật bài viết thành công.",
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

		// Admin xoá bài viết
		[HttpDelete("delete/{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			try
			{
				var result = await _postService.AdminDeleteAsync(id, userId);
				return Ok(new
				{
					message = "Xóa bài viết thành công.",
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

		// Admin duyệt bài viết
		[HttpPost("{id}/approve")]
		public async Task<IActionResult> Approve(int id)
		{
			if (!TryGetCurrentUserId(out var adminId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			try
			{
				var result = await _postService.ApproveAsync(id, adminId);
				return Ok(new
				{
					message = "Duyệt bài viết thành công.",
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

		// Admin từ chối bài viết
		[HttpPost("{id}/reject")]
		public async Task<IActionResult> Reject(int id, [FromBody] RejectPostRequest request)
		{
			if (!TryGetCurrentUserId(out var adminId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			try
			{
				var result = await _postService.RejectAsync(id, adminId, request.RejectReason);
				return Ok(new
				{
					message = "Từ chối bài viết thành công.",
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

		// Lấy danh sách bài viết chờ duyệt (Pending) của một CLB - cho nhóm trưởng hoặc admin
		[HttpGet("clubs/{clubId}/pending-posts")]
		public async Task<IActionResult> GetPendingPostsByClub(int clubId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
		{
			var request = new PostSearchRequest
			{
				ClubId = clubId,
				Status = "Pending",
				Page = page,
				PageSize = pageSize
			};

			var result = await _postService.GetAllPostsAsync(request);
			return Ok(result);
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
	/// <summary>
		/// Lấy tất cả bài viết chờ duyệt (pending)
		/// </summary>
		[HttpGet("pending")]
		public async Task<IActionResult> GetPendingPosts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
		{
			var request = new PostSearchRequest
			{
				Status = "Pending",
				Page = page,
				PageSize = pageSize
			};
			var result = await _postService.GetAllPostsAsync(request);
			return Ok(result);
		}

		/// <summary>
		/// Lấy tất cả bài viết đã duyệt (approved)
		/// </summary>
		[HttpGet("approved")]
		public async Task<IActionResult> GetApprovedPosts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
		{
			var request = new PostSearchRequest
			{
				Status = "Approved",
				Page = page,
				PageSize = pageSize
			};
			var result = await _postService.GetAllPostsAsync(request);
			return Ok(result);
		}

		/// <summary>
		/// Lấy tất cả bài viết đã từ chối (rejected)
		/// </summary>
		[HttpGet("rejected")]
		public async Task<IActionResult> GetRejectedPosts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
		{
			var request = new PostSearchRequest
			{
				Status = "Rejected",
				Page = page,
				PageSize = pageSize
			};
			var result = await _postService.GetAllPostsAsync(request);
			return Ok(result);
		}
	}
}