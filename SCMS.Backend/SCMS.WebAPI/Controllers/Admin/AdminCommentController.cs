using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.DTOs.Search;
using SCMS.Contracts.Interfaces.iService;
using SCMS.DomainEntities.Entities;
using SCMS.DomainEntities.Enums; 
using SCMS.WebAPI.Attributes; // using directive for the custom PermissionAttribute

namespace SCMS.WebAPI.Controllers.Admin
{
	[ApiController]
	[Route("api/admin/comments")]
	public class AdminCommentController : ControllerBase
	{
		private readonly ICommentService _commentService;

		public AdminCommentController(ICommentService commentService)
		{
			_commentService = commentService;
		}

		[Permission(AppPermission.Admin_Comment_View_List)]
		// Admin xem tất cả bình luận (search + pagination)
		[HttpGet("list")]
		public async Task<IActionResult> GetAll([FromQuery] CommentSearchRequest request)
		{
			request ??= new CommentSearchRequest();
			if (request.Page < 1) request.Page = 1;
			if (request.PageSize < 1) request.PageSize = 10;

			var result = await _commentService.GetAllCommentsAsync(request);
			return Ok(result);
		}

		[Permission(AppPermission.Admin_Comment_View_Detail)]
		// Admin xem chi tiết một bình luận
		[HttpGet("detail/{id}")]
		public async Task<IActionResult> GetDetail(int id)
		{
			try
			{
				var result = await _commentService.GetCommentDetailAsync(id);
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
		[Permission(AppPermission.Admin_Comment_Update)]
		// Admin cập nhật bình luận (phần này cần chuẩn hoá nếu cần lưu vào DB)
		[HttpPut("update/{id}")]
		public async Task<IActionResult> Update(int id, [FromBody] UpdateCommentRequest request)
		{
			if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			request.CommentId = id;

			try
			{
				var result = await _commentService.AdminUpdateAsync(request, userId);
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

		[Permission(AppPermission.Admin_Comment_Delete_Any)]
		// Admin xóa bình luận
		[HttpDelete("delete/{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			if (!TryGetCurrentUserId(out var userId, out var unauthorizedResult))
			{
				return unauthorizedResult!;
			}

			try
			{
				var result = await _commentService.AdminDeleteAsync(id, userId);
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

		// Lấy tất cả bình luận của một CLB (lọc theo clubId)
		[Permission(AppPermission.Admin_Comment_View_By_Club)]
		[HttpGet("clubs/{clubId}/comments")]
	
		public async Task<IActionResult> GetAllByClub(int clubId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
		{
			var request = new CommentSearchRequest
			{
				ClubId = clubId,
				Page = page,
				PageSize = pageSize
			};
			var result = await _commentService.GetAllCommentsAsync(request);
			return Ok(result);
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
