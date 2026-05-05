using System.Collections.Generic;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.DomainEntities.Entities;

namespace SCMS.Contracts.Interfaces.iRepositores
{
	// Interface thao tác dữ liệu feedback
	public interface IFeedbackRepository
	{
		// Tạo mới feedback
		Task<Feedback> AddAsync(Feedback feedback);

		// Cập nhật feedback
		Task<Feedback> UpdateAsync(Feedback feedback);

		// Xóa feedback
		Task DeleteAsync(Feedback feedback);

		// Lấy feedback theo id
		Task<Feedback?> GetByIdAsync(int feedbackId);

		// Lấy feedback theo id và userId để kiểm tra quyền sở hữu
		Task<Feedback?> GetByIdAsync(int feedbackId, int userId);

		// Lấy feedback dạng tracked theo id để cập nhật/xóa (admin)
		Task<Feedback?> GetByIdForUpdateAsync(int feedbackId);

		// Lấy feedback dạng tracked theo id + userId để cập nhật/xóa (user)
		Task<Feedback?> GetByIdForUpdateAsync(int feedbackId, int userId);

		// Lấy danh sách feedback của user, có tìm kiếm và phân trang
		Task<(List<Feedback> Items, int TotalCount)> GetMyFeedbacksAsync(
			int userId,
			FeedbackSearchRequest request);

		// Lấy danh sách tất cả feedback, có tìm kiếm và phân trang cho admin
		Task<(List<Feedback> Items, int TotalCount)> GetAllFeedbacksAsync(FeedbackSearchRequest request);
	}
}
