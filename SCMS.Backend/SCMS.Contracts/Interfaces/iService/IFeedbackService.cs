using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.DTOs.Responses;

namespace SCMS.Contracts.Interfaces.iService
{
	// Interface nghiệp vụ cho feedback
	public interface IFeedbackService
	{
		// User tạo feedback mới
		Task<FeedbackDetailResponse> CreateAsync(CreateFeedbackRequest request, int userId);

		// User cập nhật feedback của chính mình
		Task<FeedbackDetailResponse> UpdateAsync(UpdateFeedbackRequest request, int userId);

		// User xóa feedback của chính mình
		Task<FeedbackDetailResponse> DeleteAsync(int feedbackId, int userId);

		// User xem danh sách feedback của mình, có tìm kiếm và phân trang
		Task<PagedResult<FeedbackResponse>> GetMyFeedbacksAsync(int userId, FeedbackSearchRequest request);

		// User xem chi tiết một feedback của mình
		Task<FeedbackDetailResponse> GetMyFeedbackDetailAsync(int feedbackId, int userId);

		// Admin xem danh sách tất cả feedback, có tìm kiếm và phân trang
		Task<PagedResult<FeedbackResponse>> GetAllFeedbacksAsync(FeedbackSearchRequest request);

		// Admin xem chi tiết một feedback
		Task<FeedbackDetailResponse> GetFeedbackDetailAsync(int feedbackId);

		// Admin cập nhật feedback
		Task<FeedbackDetailResponse> AdminUpdateAsync(UpdateFeedbackRequest request, int adminId);

		// Admin xóa feedback
		Task<FeedbackDetailResponse> AdminDeleteAsync(int feedbackId, int adminId);

		// Admin đánh dấu feedback đã xem
		Task<FeedbackDetailResponse> MarkReviewedAsync(int feedbackId, int adminId);

		// Admin đánh dấu feedback đã giải quyết
		Task<FeedbackDetailResponse> MarkResolvedAsync(int feedbackId, int adminId);
	}
}
