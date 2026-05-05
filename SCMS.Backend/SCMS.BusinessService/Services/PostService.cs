
	
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.DTOs.Responses;
using SCMS.Contracts.Interfaces.iRepositores;
using SCMS.Contracts.Interfaces.iService;
using SCMS.DomainEntities.Entities;

namespace SCMS.BusinessService.Services
{
	// Triển khai nghiệp vụ bài viết cho user
	public class PostService : IPostService
	{
		private readonly IPostRepository _postRepository;
		private readonly IClubRepository _clubRepository;
		private readonly IEventRepository _eventRepository;
		private readonly IMembershipRepository _membershipRepository;
		private readonly IAuditLogService _auditLogService;

		public PostService(
			IPostRepository postRepository,
			IClubRepository clubRepository,
			IEventRepository eventRepository,
			IMembershipRepository membershipRepository,
			IAuditLogService auditLogService)
		{
			_postRepository = postRepository;
			_clubRepository = clubRepository;
			_eventRepository = eventRepository;
			_membershipRepository = membershipRepository;
			_auditLogService = auditLogService;
		}

		// User tạo bài viết mới
		public async Task<PostDetailResponse> CreateAsync(CreatePostRequest request, int userId)
		{
			ValidatePostInput(request.Title, request.Content);

			await ValidateUserClubAndEventAccessAsync(userId, request.ClubId, request.EventId);

			var post = new Post
			{
				Title = request.Title.Trim(),
				Content = request.Content.Trim(),
				ClubId = request.ClubId,
				EventId = request.EventId,
				UserId = userId,
				PostType = request.PostType?.Trim(),
				Status = "Pending",
				RejectReason = null,
				CreatedAt = DateTime.UtcNow
			};

			var created = await _postRepository.AddAsync(post);
			var detail = await _postRepository.GetByIdAsync(created.PostId);

			// User tạo bài viết mới.
			await SafeWriteAuditLogAsync(userId, "Post", created.PostId, "CREATE");

			return MapToDetailResponse(detail ?? created);
		}

		// Admin tạo bài viết mới và duyệt luôn
		public async Task<PostDetailResponse> AdminCreateAsync(CreatePostRequest request, int userId)
		{
			ValidatePostInput(request.Title, request.Content);

			await ValidateClubAndEventAsync(request.ClubId, request.EventId);

			var post = new Post
			{
				Title = request.Title.Trim(),
				Content = request.Content.Trim(),
				ClubId = request.ClubId,
				EventId = request.EventId,
				UserId = userId,
				PostType = request.PostType?.Trim(),
				Status = "Approved",
				RejectReason = null,
				CreatedAt = DateTime.UtcNow
			};

			var created = await _postRepository.AddAsync(post);
			var detail = await _postRepository.GetByIdAsync(created.PostId);

			// Admin tạo bài viết trực tiếp.
			await SafeWriteAuditLogAsync(userId, "Post", created.PostId, "ADMIN_CREATE");

			return MapToDetailResponse(detail ?? created);
		}

		// User cập nhật bài viết của chính mình
		public async Task<PostDetailResponse> UpdateAsync(UpdatePostRequest request, int userId)
		{
			ValidatePostInput(request.Title, request.Content);

			await ValidateClubAndEventAsync(request.ClubId, request.EventId);

			var post = await _postRepository.GetByIdForUpdateAsync(request.PostId, userId);
			if (post == null)
			{
				throw new KeyNotFoundException("Không tìm thấy bài viết hoặc bạn không có quyền sửa bài này.");
			}

			if (string.Equals(post.Status, "Deleted", StringComparison.OrdinalIgnoreCase))
			{
				throw new InvalidOperationException("Không thể sửa bài viết đã bị xóa.");
			}

			post.Title = request.Title.Trim();
			post.Content = request.Content.Trim();
			post.ClubId = request.ClubId;
			post.EventId = request.EventId;
			post.PostType = request.PostType?.Trim();
			post.Status = "Pending";
			post.RejectReason = null;

			var updated = await _postRepository.UpdateAsync(post);
			var detail = await _postRepository.GetByIdAsync(updated.PostId, userId);

			await SafeWriteAuditLogAsync(userId, "Post", updated.PostId, "UPDATE");

			return MapToDetailResponse(detail ?? updated);
		}

		// Admin cập nhật bài viết
		public async Task<PostDetailResponse> AdminUpdateAsync(UpdatePostRequest request, int userId)
		{
			ValidatePostInput(request.Title, request.Content);

			await ValidateClubAndEventAsync(request.ClubId, request.EventId);

			var post = await _postRepository.GetByIdForUpdateAsync(request.PostId);
			if (post == null)
			{
				throw new KeyNotFoundException("Không tìm thấy bài viết cần cập nhật.");
			}

			if (string.Equals(post.Status, "Deleted", StringComparison.OrdinalIgnoreCase))
			{
				throw new InvalidOperationException("Không thể cập nhật bài viết đã bị xóa.");
			}

			post.Title = request.Title.Trim();
			post.Content = request.Content.Trim();
			post.ClubId = request.ClubId;
			post.EventId = request.EventId;
			post.PostType = request.PostType?.Trim();
			post.Status = "Approved";
			post.RejectReason = null;

			var updated = await _postRepository.UpdateAsync(post);
			var detail = await _postRepository.GetByIdAsync(updated.PostId);

			await SafeWriteAuditLogAsync(userId, "Post", updated.PostId, "ADMIN_UPDATE");

			return MapToDetailResponse(detail ?? updated);
		}

		// User xóa bài viết của chính mình
		public async Task<PostDetailResponse> DeleteAsync(int postId, int userId)
		{
			var post = await _postRepository.GetByIdForUpdateAsync(postId, userId);
			if (post == null)
			{
				throw new KeyNotFoundException("Không tìm thấy bài viết hoặc bạn không có quyền xóa bài này.");
			}

			post.Status = "Deleted";
			post.RejectReason = null;

			var updated = await _postRepository.UpdateAsync(post);
			var detail = await _postRepository.GetByIdAsync(updated.PostId, userId);

			await SafeWriteAuditLogAsync(userId, "Post", updated.PostId, "DELETE");

			return MapToDetailResponse(detail ?? updated);
		}
	// User xem danh sách bài viết đã like
		public async Task<PagedResult<PostResponse>> GetLikedPostsAsync(int userId, PostSearchRequest request)
		{
			request ??= new PostSearchRequest();
			var (items, totalCount) = await _postRepository.GetLikedPostsAsync(userId, request);
			return new PagedResult<PostResponse>
			{
				Items = items.Select(MapToResponse).ToList(),
				TotalCount = totalCount,
				Page = request.Page < 1 ? 1 : request.Page,
				PageSize = request.PageSize < 1 ? 10 : request.PageSize
			};
		}
		// Admin xoá bài viết
		public async Task<PostDetailResponse> AdminDeleteAsync(int postId, int userId)
		{
			var post = await _postRepository.GetByIdForUpdateAsync(postId);
			if (post == null)
			{
				throw new KeyNotFoundException("Không tìm thấy bài viết cần xóa.");
			}

			if (string.Equals(post.Status, "Deleted", StringComparison.OrdinalIgnoreCase))
			{
				throw new InvalidOperationException("Bài viết đã bị xóa trước đó.");
			}

			post.Status = "Deleted";
			post.RejectReason = null;

			var updated = await _postRepository.UpdateAsync(post);
			var detail = await _postRepository.GetByIdAsync(updated.PostId);

			await SafeWriteAuditLogAsync(userId, "Post", updated.PostId, "ADMIN_DELETE");

			return MapToDetailResponse(detail ?? updated);
		}

		// User xem danh sách bài viết của mình, có tìm kiếm và phân trang
		public async Task<PagedResult<PostResponse>> GetMyPostsAsync(int userId, PostSearchRequest request)
		{
			request ??= new PostSearchRequest();

			var (items, totalCount) = await _postRepository.GetMyPostsAsync(userId, request);

			return new PagedResult<PostResponse>
			{
				Items = items.Select(MapToResponse).ToList(),
				TotalCount = totalCount,
				Page = request.Page < 1 ? 1 : request.Page,
				PageSize = request.PageSize < 1 ? 10 : request.PageSize
			};
		}

		// User xem danh sách bài viết của các CLB mình đã tham gia
		public async Task<PagedResult<PostResponse>> GetMyClubPostsAsync(int userId, PostSearchRequest request)
		{
			request ??= new PostSearchRequest();

			var clubIds = await _membershipRepository.GetApprovedClubIdsByUserAsync(userId);
			if (clubIds.Count == 0)
			{
				return new PagedResult<PostResponse>
				{
					Items = new List<PostResponse>(),
					TotalCount = 0,
					Page = request.Page < 1 ? 1 : request.Page,
					PageSize = request.PageSize < 1 ? 10 : request.PageSize
				};
			}

			var (items, totalCount) = await _postRepository.GetMyClubPostsAsync(clubIds, request);

			return new PagedResult<PostResponse>
			{
				Items = items.Select(MapToResponse).ToList(),
				TotalCount = totalCount,
				Page = request.Page < 1 ? 1 : request.Page,
				PageSize = request.PageSize < 1 ? 10 : request.PageSize
			};
		}

		// User xem danh sách bài viết công khai toàn hệ thống
		public async Task<PagedResult<PostResponse>> GetPublicPostsAsync(PostSearchRequest request)
		{
			request ??= new PostSearchRequest();

			var (items, totalCount) = await _postRepository.GetPublicPostsAsync(request);

			return new PagedResult<PostResponse>
			{
				Items = items.Select(MapToResponse).ToList(),
				TotalCount = totalCount,
				Page = request.Page < 1 ? 1 : request.Page,
				PageSize = request.PageSize < 1 ? 10 : request.PageSize
			};
		}

		// User xem chi tiết một bài viết của mình
		public async Task<PostDetailResponse> GetMyPostDetailAsync(int postId, int userId)
		{
			var post = await _postRepository.GetByIdAsync(postId, userId);
			if (post == null)
			{
				throw new KeyNotFoundException("Không tìm thấy bài viết hoặc bạn không có quyền xem bài này.");
			}

			return MapToDetailResponse(post);
		}

		// Admin xem danh sách tất cả bài viết, có tìm kiếm và phân trang
		public async Task<PagedResult<PostResponse>> GetAllPostsAsync(PostSearchRequest request)
		{
			request ??= new PostSearchRequest();

			var (items, totalCount) = await _postRepository.GetAllPostsAsync(request);

			return new PagedResult<PostResponse>
			{
				Items = items.Select(MapToResponse).ToList(),
				TotalCount = totalCount,
				Page = request.Page < 1 ? 1 : request.Page,
				PageSize = request.PageSize < 1 ? 10 : request.PageSize
			};
		}

		// Admin xem chi tiết một bài viết
		public async Task<PostDetailResponse> GetPostDetailAsync(int postId)
		{
			var post = await _postRepository.GetByIdAsync(postId);
			if (post == null)
			{
				throw new KeyNotFoundException("Không tìm thấy bài viết.");
			}

			return MapToDetailResponse(post);
		}

		// Admin duyệt bài viết
		public async Task<PostDetailResponse> ApproveAsync(int postId, int adminId)
		{
			var post = await _postRepository.GetByIdForUpdateAsync(postId);
			if (post == null)
			{
				throw new KeyNotFoundException("Không tìm thấy bài viết cần duyệt.");
			}

			if (string.Equals(post.Status, "Deleted", StringComparison.OrdinalIgnoreCase))
			{
				throw new InvalidOperationException("Không thể duyệt bài viết đã bị xóa.");
			}

			post.Status = "Approved";
			post.RejectReason = null;

			var updated = await _postRepository.UpdateAsync(post);
			var detail = await _postRepository.GetByIdAsync(updated.PostId);

			await SafeWriteAuditLogAsync(adminId, "Post", updated.PostId, "APPROVE");

			return MapToDetailResponse(detail ?? updated);
		}

		// Admin từ chối bài viết
		public async Task<PostDetailResponse> RejectAsync(int postId, int adminId, string rejectReason)
		{
			if (string.IsNullOrWhiteSpace(rejectReason))
			{
				throw new ArgumentException("RejectReason không được để trống.");
			}

			var post = await _postRepository.GetByIdForUpdateAsync(postId);
			if (post == null)
			{
				throw new KeyNotFoundException("Không tìm thấy bài viết cần từ chối.");
			}

			if (string.Equals(post.Status, "Deleted", StringComparison.OrdinalIgnoreCase))
			{
				throw new InvalidOperationException("Không thể từ chối bài viết đã bị xóa.");
			}

			post.Status = "Rejected";
			post.RejectReason = rejectReason.Trim();

			var updated = await _postRepository.UpdateAsync(post);
			var detail = await _postRepository.GetByIdAsync(updated.PostId);

			await SafeWriteAuditLogAsync(adminId, "Post", updated.PostId, "REJECT", null, rejectReason.Trim());

			return MapToDetailResponse(detail ?? updated);
		}

		// User like bài viết
		public async Task<int> LikePostAsync(int postId, int userId)
		{
			// Chỉ cho like bài viết tồn tại và chưa bị xóa
			var post = await _postRepository.GetByIdAsync(postId);
			if (post == null || string.Equals(post.Status, "Deleted", StringComparison.OrdinalIgnoreCase))
			{
				throw new KeyNotFoundException("Không tìm thấy bài viết.");
			}

			// Chặn like trùng
			var existed = await _postRepository.GetPostLikeAsync(postId, userId);
			if (existed != null)
			{
				throw new InvalidOperationException("Bạn đã like bài viết này trước đó.");
			}

			var like = new Like
			{
				UserId = userId,
				PostId = postId,
				CommentId = null,
				CreatedAt = DateTime.UtcNow
			};

			await _postRepository.AddPostLikeAsync(like);
			return await _postRepository.CountPostLikesAsync(postId);
		}

		// User bỏ like bài viết
		public async Task<int> UnlikePostAsync(int postId, int userId)
		{
			var post = await _postRepository.GetByIdAsync(postId);
			if (post == null || string.Equals(post.Status, "Deleted", StringComparison.OrdinalIgnoreCase))
			{
				throw new KeyNotFoundException("Không tìm thấy bài viết.");
			}

			var existed = await _postRepository.GetPostLikeAsync(postId, userId);
			if (existed == null)
			{
				throw new InvalidOperationException("Bạn chưa like bài viết này.");
			}

			await _postRepository.RemovePostLikeAsync(existed);
			return await _postRepository.CountPostLikesAsync(postId);
		}

		// User báo cáo bài viết
		public async Task<string> ReportPostAsync(int postId, int userId, CreatePostReportRequest request)
		{
			if (request == null || string.IsNullOrWhiteSpace(request.Reason))
			{
				throw new ArgumentException("Lý do báo cáo không được để trống.");
			}

			var post = await _postRepository.GetByIdAsync(postId);
			if (post == null || string.Equals(post.Status, "Deleted", StringComparison.OrdinalIgnoreCase))
			{
				throw new KeyNotFoundException("Không tìm thấy bài viết.");
			}

			// Không cho user tự báo cáo bài viết của chính mình
			if (post.UserId == userId)
			{
				throw new InvalidOperationException("Bạn không thể báo cáo bài viết của chính mình.");
			}

			// Chặn report trùng cho cùng post + user
			var alreadyReported = await _postRepository.HasUserReportedPostAsync(postId, userId);
			if (alreadyReported)
			{
				throw new InvalidOperationException("Bạn đã báo cáo bài viết này trước đó.");
			}

			var report = new PostReport
			{
				PostId = postId,
				UserId = userId,
				Reason = request.Reason.Trim(),
				Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
				Status = "Pending",
				CreatedAt = DateTime.UtcNow
			};

			await _postRepository.AddPostReportAsync(report);

			// User báo cáo bài viết.
			await SafeWriteAuditLogAsync(userId, "Post", postId, "REPORT", null, request.Reason.Trim());

			return "Báo cáo bài viết thành công.";
		}

		// Best effort: lỗi audit log không làm fail luồng nghiệp vụ chính.
		private async Task SafeWriteAuditLogAsync(int actorUserId, string tableName, int recordId, string actionType, string? oldValue = null, string? newValue = null)
		{
			try
			{
				await _auditLogService.LogAuditAsync(actorUserId, tableName, recordId, actionType, oldValue, newValue);
			}
			catch
			{
				// Intentionally ignore audit failures.
			}
		}

		// Kiểm tra dữ liệu bài viết cơ bản
		private static void ValidatePostInput(string title, string content)
		{
			if (string.IsNullOrWhiteSpace(title))
			{
				throw new ArgumentException("Title không được để trống.");
			}

			if (string.IsNullOrWhiteSpace(content))
			{
				throw new ArgumentException("Content không được để trống.");
			}
		}

		// Kiểm tra ClubId và EventId nếu người dùng có truyền lên
		// Với user thường, chỉ cho phép đăng bài vào CLB mình đã tham gia hoặc sự kiện thuộc CLB mình đã tham gia.
		// Nếu gửi cả ClubId và EventId thì hai giá trị phải khớp nhau.
		private async Task ValidateUserClubAndEventAccessAsync(int userId, int? clubId, int? eventId)
		{
			if (!clubId.HasValue && !eventId.HasValue)
			{
				return;
			}

			int? resolvedClubId = clubId;

			if (eventId.HasValue)
			{
				var eventEntity = await _eventRepository.GetByIdAsync(eventId.Value);
				if (eventEntity == null)
				{
					throw new KeyNotFoundException("Không tìm thấy sự kiện được chọn.");
				}

				// Event luôn thuộc về một CLB cụ thể, nên nếu client gửi ClubId thì phải khớp.
				if (resolvedClubId.HasValue && resolvedClubId.Value != eventEntity.ClubId)
				{
					throw new ArgumentException("ClubId không khớp với CLB của sự kiện được chọn.");
				}

				resolvedClubId = eventEntity.ClubId;
			}

			if (resolvedClubId.HasValue)
			{
				// User chỉ được đăng bài cho CLB mà họ đã tham gia và membership phải được duyệt.
				var approvedMembership = await _membershipRepository.GetApprovedMembershipAsync(userId, resolvedClubId.Value);
				if (approvedMembership == null)
				{
					throw new UnauthorizedAccessException("Bạn chưa tham gia CLB này nên không thể đăng bài cho CLB đó.");
				}
			}
		}

		// Kiểm tra ClubId và EventId nếu người dùng có truyền lên
		private async Task ValidateClubAndEventAsync(int? clubId, int? eventId)
		{
			if (clubId.HasValue)
			{
				var club = await _clubRepository.GetClubByIdAsync(clubId.Value);
				if (club == null)
				{
					throw new KeyNotFoundException("Không tìm thấy câu lạc bộ được chọn.");
				}
			}

			if (eventId.HasValue)
			{
				var eventEntity = await _eventRepository.GetByIdAsync(eventId.Value);
				if (eventEntity == null)
				{
					throw new KeyNotFoundException("Không tìm thấy sự kiện được chọn.");
				}
			}
		}

		// Map entity sang response danh sách
		private static PostResponse MapToResponse(Post post)
		{
			return new PostResponse
			{
				PostId = post.PostId,
				Title = post.Title,
				Content = post.Content,
				ClubId = post.ClubId,
				ClubName = post.Club?.ClubName,
				EventId = post.EventId,
				EventName = post.Event?.EventName,
				UserId = post.UserId,
				FullName = post.User?.FullName,
				PostType = post.PostType,
				Status = post.Status,
				RejectReason = post.RejectReason,
				CreatedAt = post.CreatedAt
			};
		}

		// Map entity sang response chi tiết
		private static PostDetailResponse MapToDetailResponse(Post post)
		{
			return new PostDetailResponse
			{
				PostId = post.PostId,
				Title = post.Title,
				Content = post.Content,
				ClubId = post.ClubId,
				ClubName = post.Club?.ClubName,
				EventId = post.EventId,
				EventName = post.Event?.EventName,
				UserId = post.UserId,
				FullName = post.User?.FullName,
				Email = post.User?.Email,
				PostType = post.PostType,
				Status = post.Status,
				RejectReason = post.RejectReason,
				CreatedAt = post.CreatedAt
			};
		}
	
	
	
	}
}
