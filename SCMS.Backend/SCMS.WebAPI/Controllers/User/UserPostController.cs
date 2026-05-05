using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.Interfaces.iService;
using SCMS.WebAPI.Attributes; // using directive for the custom PermissionAttribute
using SCMS.DomainEntities.Enums; // using directive for AppPermission enum

namespace SCMS.WebAPI.Controllers
{
	[ApiController]
	[Route("api/user/posts")]
	public class UserPostController : ControllerBase
	{
		private readonly IPostService _postService;

		public UserPostController(IPostService postService)
		{
			_postService = postService;
		}

		// User tạo bài viết mới
		[Permission(AppPermission.User_Post_Create)]
		[HttpPost("create")]
		public async Task<IActionResult> Create([FromBody] CreatePostRequest request)
		{
			if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			try
			{
				var result = await _postService.CreateAsync(request, userId);
				return Ok(new
				{
					message = "Tạo bài viết thành công. Bài viết đang chờ duyệt.",
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
			catch (UnauthorizedAccessException ex)
			{
				return StatusCode(403, new { message = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = ex.Message });
			}
		}

		// User xem danh sách bài viết của chính mình (search + pagination)
		[Permission(AppPermission.User_Post_View_My_List)]
		[HttpGet("my-posts")]
		public async Task<IActionResult> GetMyPosts([FromQuery] PostSearchRequest request)
		{
			if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			request ??= new PostSearchRequest();
			if (request.Page < 1) request.Page = 1;
			if (request.PageSize < 1) request.PageSize = 10;

			var result = await _postService.GetMyPostsAsync(userId, request);
			return Ok(result);
		}

		// User xem danh sách bài viết của các CLB mình đã tham gia
				[Permission(AppPermission.User_Post_View_My_Club_List)]
				[HttpGet("my-club-posts")]
				public async Task<IActionResult> GetMyClubPosts([FromQuery] PostSearchRequest request)
				{
					if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
					{
						return unauthorizedResult!;
					}

					request ??= new PostSearchRequest();
					if (request.Page < 1) request.Page = 1;
					if (request.PageSize < 1) request.PageSize = 10;

					// Nếu không truyền status thì mặc định chỉ lấy bài đã duyệt
					if (string.IsNullOrWhiteSpace(request.Status))
					{
						request.Status = "Approved";
					}

					var result = await _postService.GetMyClubPostsAsync(userId, request);
					return Ok(result);
				}

		// User xem danh sách bài viết công khai toàn hệ thống
		[Permission(AppPermission.User_Post_View_Public_List)]
		[HttpGet("public")]
		public async Task<IActionResult> GetPublicPosts([FromQuery] PostSearchRequest request)
		{
			request ??= new PostSearchRequest();
			if (request.Page < 1) request.Page = 1;
			if (request.PageSize < 1) request.PageSize = 10;

			var result = await _postService.GetPublicPostsAsync(request);
			return Ok(result);
		}

		// User xem chi tiết một bài viết của chính mình
		[Permission(AppPermission.User_Post_View_My_Detail)]
		[HttpGet("my-posts/{id}")]
		public async Task<IActionResult> GetMyPostDetail(int id)
		{
			if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			try
			{
				var result = await _postService.GetMyPostDetailAsync(id, userId);
				return Ok(result);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(new { message = ex.Message });
			}
		}

		// User sửa bài viết của chính mình
		[Permission(AppPermission.User_Post_Update)]
		[HttpPut("my-posts/{id}")] // id của bài viết cần sửa
		public async Task<IActionResult> Update(int id, [FromBody] UpdatePostRequest request)
		{
			if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			request.PostId = id;

			try
			{
				var result = await _postService.UpdateAsync(request, userId);
				return Ok(new
				{
					message = "Cập nhật bài viết thành công. Bài viết đang chờ duyệt lại.",
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

		// User xóa bài viết của chính mình
		[Permission(AppPermission.User_Post_Delete)]
		[HttpDelete("my-posts/{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			try
			{
				var result = await _postService.DeleteAsync(id, userId);
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

		// User like bài viết
		[Permission(AppPermission.User_Post_Like)]
		[HttpPost("{id}/like")]
		public async Task<IActionResult> LikePost(int id)
		{
			if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			try
			{
				var likeCount = await _postService.LikePostAsync(id, userId);
				return Ok(new
				{
					message = "Like bài viết thành công.",
					postId = id,
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

		// User bỏ like bài viết
		[Permission(AppPermission.User_Post_Unlike)]
		[HttpDelete("{id}/like")]
		public async Task<IActionResult> UnlikePost(int id)
		{
			if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			try
			{
				var likeCount = await _postService.UnlikePostAsync(id, userId);
				return Ok(new
				{
					message = "Bỏ like bài viết thành công.",
					postId = id,
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
		

		// User báo cáo bài viết
		[Permission(AppPermission.User_Post_Report)]
		[HttpPost("{id}/report")]
		public async Task<IActionResult> ReportPost(int id, [FromBody] CreatePostReportRequest request)
		{
			if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			try
			{
				var message = await _postService.ReportPostAsync(id, userId, request);
				return Ok(new
				{
					message,
					postId = id
				});
			}
			catch (ArgumentException ex)
			{
				return BadRequest(new { message = ex.Message });
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
		
		// User xem danh sách bài viết đã like
		[Permission(AppPermission.User_Post_View_Liked_List)]
[HttpGet("liked-posts")]
public async Task<IActionResult> GetLikedPosts([FromQuery] PostSearchRequest request)
{
    if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
    {
        return unauthorizedResult!;
    }

    request ??= new PostSearchRequest();
    if (request.Page < 1) request.Page = 1;
    if (request.PageSize < 1) request.PageSize = 10;

    var result = await _postService.GetLikedPostsAsync(userId, request);
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
	}
}
