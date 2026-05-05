using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.DTOs.Responses;
using SCMS.Contracts.DTOs.Search;
using SCMS.Contracts.Interfaces.iRepositores;
using SCMS.Contracts.Interfaces.iService;
using SCMS.DomainEntities.Entities;

namespace SCMS.BusinessService.Services
{
	// Triển khai nghiệp vụ bình luận
	public class CommentService : ICommentService
	{
		private readonly ICommentRepository _commentRepository;
		private readonly IPostRepository _postRepository;

		public CommentService(
			ICommentRepository commentRepository,
			IPostRepository postRepository)
		{
			_commentRepository = commentRepository;
			_postRepository = postRepository;
		}

		// User tạo bình luận mới
		public async Task<CommentDetailResponse> CreateAsync(CreateCommentRequest request, int userId)
		{
			ValidateCommentInput(request.Content);

			// Kiểm tra bài viết tồn tại
			var post = await _postRepository.GetByIdAsync(request.PostId);
			if (post == null)
			{
				throw new KeyNotFoundException("Không tìm thấy bài viết này.");
			}

			// Chặn bình luận nếu bài viết chưa được duyệt
			if (!string.Equals(post.Status, "Approved", StringComparison.OrdinalIgnoreCase))
			{
				throw new ArgumentException("Không thể bình luận vào bài viết chưa được duyệt.");
			}

			var comment = new Comment
			{
				PostId = request.PostId,
				UserId = userId,
				Content = request.Content.Trim(),
				CreatedAt = DateTime.UtcNow
			};

			var created = await _commentRepository.AddAsync(comment);
			var detail = await _commentRepository.GetByIdAsync(created.CommentId);

			return MapToDetailResponse(detail ?? created);
		}

		// User cập nhật bình luận của chính mình
		public async Task<CommentDetailResponse> UpdateAsync(UpdateCommentRequest request, int userId)
		{
			ValidateCommentInput(request.Content);

			var comment = await _commentRepository.GetByIdForUpdateAsync(request.CommentId, userId);
			if (comment == null)
			{
				throw new KeyNotFoundException("Không tìm thấy bình luận hoặc bạn không có quyền sửa bình luận này.");
			}

			comment.Content = request.Content.Trim();

			var updated = await _commentRepository.UpdateAsync(comment);
			var detail = await _commentRepository.GetByIdAsync(updated.CommentId);

			return MapToDetailResponse(detail ?? updated);
		}

		// User xóa bình luận của chính mình
		public async Task<CommentDetailResponse> DeleteAsync(int commentId, int userId)
		{
			var comment = await _commentRepository.GetByIdForUpdateAsync(commentId, userId);
			if (comment == null)
			{
				throw new KeyNotFoundException("Không tìm thấy bình luận hoặc bạn không có quyền xóa bình luận này.");
			}

			// Trả về detail trước khi xóa để response
			var detail = await _commentRepository.GetByIdAsync(commentId);
			var response = MapToDetailResponse(detail ?? comment);

			// Xóa bình luận
			await _commentRepository.DeleteAsync(comment);

			return response;
		}

		// User xem danh sách bình luận của mình, có tìm kiếm và phân trang
		public async Task<PagedResult<CommentResponse>> GetMyCommentsAsync(int userId, CommentSearchRequest request)
		{
			request ??= new CommentSearchRequest();
			if (request.Page < 1) request.Page = 1;
			if (request.PageSize < 1) request.PageSize = 10;

			var (items, totalCount) = await _commentRepository.GetMyCommentsAsync(userId, request);

			var responses = items.Select(MapToResponse).ToList();

			return new PagedResult<CommentResponse>
			{
				Items = responses,
				TotalCount = totalCount,
				Page = request.Page,
				PageSize = request.PageSize
			};
		}

		// User xem chi tiết một bình luận của mình
		public async Task<CommentDetailResponse> GetMyCommentDetailAsync(int commentId, int userId)
		{
			var comment = await _commentRepository.GetByIdAsync(commentId, userId);
			if (comment == null)
			{
				throw new KeyNotFoundException("Không tìm thấy bình luận hoặc bạn không có quyền xem bình luận này.");
			}

			return MapToDetailResponse(comment);
		}

		// Admin xem danh sách tất cả bình luận, có tìm kiếm và phân trang
		public async Task<PagedResult<CommentResponse>> GetAllCommentsAsync(CommentSearchRequest request)
		{
			request ??= new CommentSearchRequest();
			if (request.Page < 1) request.Page = 1;
			if (request.PageSize < 1) request.PageSize = 10;

			var (items, totalCount) = await _commentRepository.GetAllCommentsAsync(request);

			var responses = items.Select(MapToResponse).ToList();

			return new PagedResult<CommentResponse>
			{
				Items = responses,
				TotalCount = totalCount,
				Page = request.Page,
				PageSize = request.PageSize
			};
		}

		// Admin xem chi tiết một bình luận
		public async Task<CommentDetailResponse> GetCommentDetailAsync(int commentId)
		{
			var comment = await _commentRepository.GetByIdAsync(commentId);
			if (comment == null)
			{
				throw new KeyNotFoundException("Không tìm thấy bình luận này.");
			}

			return MapToDetailResponse(comment);
		}

		// Admin cập nhật bình luận
		public async Task<CommentDetailResponse> AdminUpdateAsync(UpdateCommentRequest request, int userId)
		{
			ValidateCommentInput(request.Content);

			var comment = await _commentRepository.GetByIdForUpdateAsync(request.CommentId);
			if (comment == null)
			{
				throw new KeyNotFoundException("Không tìm thấy bình luận này.");
			}

			comment.Content = request.Content.Trim();

			var updated = await _commentRepository.UpdateAsync(comment);
			var detail = await _commentRepository.GetByIdAsync(updated.CommentId);

			return MapToDetailResponse(detail ?? updated);
		}

		// Admin xóa bình luận
		public async Task<CommentDetailResponse> AdminDeleteAsync(int commentId, int userId)
		{
			var comment = await _commentRepository.GetByIdForUpdateAsync(commentId);
			if (comment == null)
			{
				throw new KeyNotFoundException("Không tìm thấy bình luận này.");
			}

			// Trả về detail trước khi xóa để response
			var detail = await _commentRepository.GetByIdAsync(commentId);
			var response = MapToDetailResponse(detail ?? comment);

			// Xóa bình luận
			await _commentRepository.DeleteAsync(comment);

			return response;
		}

		// User like bình luận
		public async Task<int> LikeCommentAsync(int commentId, int userId)
		{
			var comment = await _commentRepository.GetByIdAsync(commentId);
			if (comment == null)
			{
				throw new KeyNotFoundException("Không tìm thấy bình luận.");
			}

			// Chặn like trùng
			var existed = await _commentRepository.GetCommentLikeAsync(commentId, userId);
			if (existed != null)
			{
				throw new InvalidOperationException("Bạn đã like bình luận này trước đó.");
			}

			var like = new Like
			{
				UserId = userId,
				PostId = null,
				CommentId = commentId,
				CreatedAt = DateTime.UtcNow
			};

			await _commentRepository.AddCommentLikeAsync(like);
			return await _commentRepository.CountCommentLikesAsync(commentId);
		}

		// User bỏ like bình luận
		public async Task<int> UnlikeCommentAsync(int commentId, int userId)
		{
			var comment = await _commentRepository.GetByIdAsync(commentId);
			if (comment == null)
			{
				throw new KeyNotFoundException("Không tìm thấy bình luận.");
			}

			var existed = await _commentRepository.GetCommentLikeAsync(commentId, userId);
			if (existed == null)
			{
				throw new InvalidOperationException("Bạn chưa like bình luận này.");
			}

			await _commentRepository.RemoveCommentLikeAsync(existed);
			return await _commentRepository.CountCommentLikesAsync(commentId);
		}

		// Helper: Validate nội dung bình luận
		private void ValidateCommentInput(string content)
		{
			if (string.IsNullOrWhiteSpace(content))
			{
				throw new ArgumentException("Nội dung bình luận không được để trống.");
			}

			if (content.Trim().Length < 1)
			{
				throw new ArgumentException("Nội dung bình luận không được để trống.");
			}

			if (content.Trim().Length > 5000)
			{
				throw new ArgumentException("Nội dung bình luận không được vượt quá 5000 ký tự.");
			}
		}

		// Helper: Map Comment entity to CommentResponse
		private CommentResponse MapToResponse(Comment comment)
		{
			return new CommentResponse
			{
				CommentId = comment.CommentId,
				PostId = comment.PostId,
				UserId = comment.UserId,
				FullName = comment.User?.FullName,
				Email = comment.User?.Email,
				Content = comment.Content,
				CreatedAt = comment.CreatedAt,
				LikeCount = comment.Likes?.Count ?? 0
			};
		}

		// Helper: Map Comment entity to CommentDetailResponse
		private CommentDetailResponse MapToDetailResponse(Comment comment)
		{
			return new CommentDetailResponse
			{
				CommentId = comment.CommentId,
				PostId = comment.PostId,
				PostTitle = comment.Post?.Title,
				UserId = comment.UserId,
				FullName = comment.User?.FullName,
				Email = comment.User?.Email,
				Content = comment.Content,
				CreatedAt = comment.CreatedAt,
				LikeCount = comment.Likes?.Count ?? 0
			};
		}
	}
}
