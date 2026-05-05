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
	// Triển khai nghiệp vụ feedback
	public class FeedbackService : IFeedbackService
	{
		private readonly IFeedbackRepository _feedbackRepository;
		private readonly IAuditLogService _auditLogService;

		public FeedbackService(IFeedbackRepository feedbackRepository, IAuditLogService auditLogService)
		{
			_feedbackRepository = feedbackRepository;
			_auditLogService = auditLogService;
		}

		// User tạo feedback mới
		public async Task<FeedbackDetailResponse> CreateAsync(CreateFeedbackRequest request, int userId)
		{
			ValidateContent(request.Content);

			var feedback = new Feedback
			{
				UserId = userId,
				Content = request.Content.Trim(),
				Status = "Pending",
				CreatedAt = DateTime.UtcNow,
				ProcessedAt = null,
				ProcessedBy = null
			};

			var created = await _feedbackRepository.AddAsync(feedback);
			var detail = await _feedbackRepository.GetByIdAsync(created.FeedbackId);

			// User tạo feedback mới.
			await SafeWriteAuditLogAsync(userId, "Feedback", created.FeedbackId, "CREATE");

			return MapToDetailResponse(detail ?? created);
		}

		// User cập nhật feedback của chính mình
		public async Task<FeedbackDetailResponse> UpdateAsync(UpdateFeedbackRequest request, int userId)
		{
			ValidateContent(request.Content);

			var feedback = await _feedbackRepository.GetByIdForUpdateAsync(request.FeedbackId, userId);
			if (feedback == null)
			{
				throw new KeyNotFoundException("Không tìm thấy feedback hoặc bạn không có quyền sửa feedback này.");
			}

			if (!string.Equals(feedback.Status, "Pending", StringComparison.OrdinalIgnoreCase))
			{
				throw new InvalidOperationException("Chỉ có thể sửa feedback khi đang ở trạng thái Pending.");
			}

			feedback.Content = request.Content.Trim();

			var updated = await _feedbackRepository.UpdateAsync(feedback);
			var detail = await _feedbackRepository.GetByIdAsync(updated.FeedbackId);

			await SafeWriteAuditLogAsync(userId, "Feedback", updated.FeedbackId, "UPDATE");

			return MapToDetailResponse(detail ?? updated);
		}

		// User xóa feedback của chính mình
		public async Task<FeedbackDetailResponse> DeleteAsync(int feedbackId, int userId)
		{
			var feedback = await _feedbackRepository.GetByIdForUpdateAsync(feedbackId, userId);
			if (feedback == null)
			{
				throw new KeyNotFoundException("Không tìm thấy feedback hoặc bạn không có quyền xóa feedback này.");
			}

			if (!string.Equals(feedback.Status, "Pending", StringComparison.OrdinalIgnoreCase))
			{
				throw new InvalidOperationException("Chỉ có thể xóa feedback khi đang ở trạng thái Pending.");
			}

			var detail = await _feedbackRepository.GetByIdAsync(feedbackId);
			var response = MapToDetailResponse(detail ?? feedback);

			await _feedbackRepository.DeleteAsync(feedback);
			await SafeWriteAuditLogAsync(userId, "Feedback", feedbackId, "DELETE");

			return response;
		}

		// User xem danh sách feedback của mình, có tìm kiếm và phân trang
		public async Task<PagedResult<FeedbackResponse>> GetMyFeedbacksAsync(int userId, FeedbackSearchRequest request)
		{
			request ??= new FeedbackSearchRequest();
			if (request.Page < 1) request.Page = 1;
			if (request.PageSize < 1) request.PageSize = 10;

			var (items, totalCount) = await _feedbackRepository.GetMyFeedbacksAsync(userId, request);

			return new PagedResult<FeedbackResponse>
			{
				Items = items.Select(MapToResponse).ToList(),
				TotalCount = totalCount,
				Page = request.Page,
				PageSize = request.PageSize
			};
		}

		// User xem chi tiết một feedback của mình
		public async Task<FeedbackDetailResponse> GetMyFeedbackDetailAsync(int feedbackId, int userId)
		{
			var feedback = await _feedbackRepository.GetByIdAsync(feedbackId, userId);
			if (feedback == null)
			{
				throw new KeyNotFoundException("Không tìm thấy feedback hoặc bạn không có quyền xem feedback này.");
			}

			return MapToDetailResponse(feedback);
		}

		// Admin xem danh sách tất cả feedback, có tìm kiếm và phân trang
		public async Task<PagedResult<FeedbackResponse>> GetAllFeedbacksAsync(FeedbackSearchRequest request)
		{
			request ??= new FeedbackSearchRequest();
			if (request.Page < 1) request.Page = 1;
			if (request.PageSize < 1) request.PageSize = 10;

			var (items, totalCount) = await _feedbackRepository.GetAllFeedbacksAsync(request);

			return new PagedResult<FeedbackResponse>
			{
				Items = items.Select(MapToResponse).ToList(),
				TotalCount = totalCount,
				Page = request.Page,
				PageSize = request.PageSize
			};
		}

		// Admin xem chi tiết một feedback
		public async Task<FeedbackDetailResponse> GetFeedbackDetailAsync(int feedbackId)
		{
			var feedback = await _feedbackRepository.GetByIdAsync(feedbackId);
			if (feedback == null)
			{
				throw new KeyNotFoundException("Không tìm thấy feedback này.");
			}

			return MapToDetailResponse(feedback);
		}

		// Admin cập nhật feedback
		public async Task<FeedbackDetailResponse> AdminUpdateAsync(UpdateFeedbackRequest request, int adminId)
		{
			ValidateContent(request.Content);

			var feedback = await _feedbackRepository.GetByIdForUpdateAsync(request.FeedbackId);
			if (feedback == null)
			{
				throw new KeyNotFoundException("Không tìm thấy feedback cần cập nhật.");
			}

			feedback.Content = request.Content.Trim();

			var updated = await _feedbackRepository.UpdateAsync(feedback);
			var detail = await _feedbackRepository.GetByIdAsync(updated.FeedbackId);

			await SafeWriteAuditLogAsync(adminId, "Feedback", updated.FeedbackId, "ADMIN_UPDATE");

			return MapToDetailResponse(detail ?? updated);
		}

		// Admin xóa feedback
		public async Task<FeedbackDetailResponse> AdminDeleteAsync(int feedbackId, int adminId)
		{
			var feedback = await _feedbackRepository.GetByIdForUpdateAsync(feedbackId);
			if (feedback == null)
			{
				throw new KeyNotFoundException("Không tìm thấy feedback cần xóa.");
			}

			var detail = await _feedbackRepository.GetByIdAsync(feedbackId);
			var response = MapToDetailResponse(detail ?? feedback);

			await _feedbackRepository.DeleteAsync(feedback);
			await SafeWriteAuditLogAsync(adminId, "Feedback", feedbackId, "ADMIN_DELETE");

			return response;
		}

		// Admin đánh dấu feedback đã xem
		public async Task<FeedbackDetailResponse> MarkReviewedAsync(int feedbackId, int adminId)
		{
			var feedback = await _feedbackRepository.GetByIdForUpdateAsync(feedbackId);
			if (feedback == null)
			{
				throw new KeyNotFoundException("Không tìm thấy feedback cần đánh dấu đã xem.");
			}

			if (string.Equals(feedback.Status, "Deleted", StringComparison.OrdinalIgnoreCase))
			{
				throw new InvalidOperationException("Không thể xử lý feedback đã bị xóa.");
			}

			feedback.Status = "Reviewed";
			feedback.ProcessedAt = DateTime.UtcNow;
			feedback.ProcessedBy = adminId;

			var updated = await _feedbackRepository.UpdateAsync(feedback);
			var detail = await _feedbackRepository.GetByIdAsync(updated.FeedbackId);

			await SafeWriteAuditLogAsync(adminId, "Feedback", updated.FeedbackId, "MARK_REVIEWED");

			return MapToDetailResponse(detail ?? updated);
		}

		// Admin đánh dấu feedback đã giải quyết
		public async Task<FeedbackDetailResponse> MarkResolvedAsync(int feedbackId, int adminId)
		{
			var feedback = await _feedbackRepository.GetByIdForUpdateAsync(feedbackId);
			if (feedback == null)
			{
				throw new KeyNotFoundException("Không tìm thấy feedback cần đánh dấu đã giải quyết.");
			}

			if (string.Equals(feedback.Status, "Deleted", StringComparison.OrdinalIgnoreCase))
			{
				throw new InvalidOperationException("Không thể xử lý feedback đã bị xóa.");
			}

			feedback.Status = "Resolved";
			feedback.ProcessedAt = DateTime.UtcNow;
			feedback.ProcessedBy = adminId;

			var updated = await _feedbackRepository.UpdateAsync(feedback);
			var detail = await _feedbackRepository.GetByIdAsync(updated.FeedbackId);

			await SafeWriteAuditLogAsync(adminId, "Feedback", updated.FeedbackId, "MARK_RESOLVED");

			return MapToDetailResponse(detail ?? updated);
		}

		// Dùng best-effort để không ảnh hưởng flow chính nếu audit log lỗi.
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

		// Helper: Validate nội dung feedback
		private void ValidateContent(string content)
		{
			if (string.IsNullOrWhiteSpace(content))
			{
				throw new ArgumentException("Nội dung feedback không được để trống.");
			}

			if (content.Trim().Length > 5000)
			{
				throw new ArgumentException("Nội dung feedback không được vượt quá 5000 ký tự.");
			}
		}

		// Helper: Map Feedback entity to FeedbackResponse
		private FeedbackResponse MapToResponse(Feedback feedback)
		{
			return new FeedbackResponse
			{
				FeedbackId = feedback.FeedbackId,
				UserId = feedback.UserId,
				FullName = feedback.User?.FullName,
				Email = feedback.User?.Email,
				Content = feedback.Content,
				Status = feedback.Status,
				CreatedAt = feedback.CreatedAt,
				ProcessedAt = feedback.ProcessedAt,
				ProcessedBy = feedback.ProcessedBy,
				ProcessedByName = feedback.ProcessedByNavigation?.FullName
			};
		}

		// Helper: Map Feedback entity to FeedbackDetailResponse
		private FeedbackDetailResponse MapToDetailResponse(Feedback feedback)
		{
			return new FeedbackDetailResponse
			{
				FeedbackId = feedback.FeedbackId,
				UserId = feedback.UserId,
				FullName = feedback.User?.FullName,
				Email = feedback.User?.Email,
				Content = feedback.Content,
				Status = feedback.Status,
				CreatedAt = feedback.CreatedAt,
				ProcessedAt = feedback.ProcessedAt,
				ProcessedBy = feedback.ProcessedBy,
				ProcessedByName = feedback.ProcessedByNavigation?.FullName
			};
		}
	}
}
