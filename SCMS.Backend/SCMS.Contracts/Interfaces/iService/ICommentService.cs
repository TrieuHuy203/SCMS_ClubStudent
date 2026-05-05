using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.DTOs.Responses;
using SCMS.Contracts.DTOs.Search;

namespace SCMS.Contracts.Interfaces.iService
{
	// Interface nghiệp vụ cho bình luận
	public interface ICommentService
	{
		// User tạo bình luận mới
		Task<CommentDetailResponse> CreateAsync(CreateCommentRequest request, int userId);

		// User cập nhật bình luận của chính mình
		Task<CommentDetailResponse> UpdateAsync(UpdateCommentRequest request, int userId);

		// User xóa bình luận của chính mình
		Task<CommentDetailResponse> DeleteAsync(int commentId, int userId);

		// User xem danh sách bình luận của mình, có tìm kiếm và phân trang
		Task<PagedResult<CommentResponse>> GetMyCommentsAsync(int userId, CommentSearchRequest request);

		// User xem chi tiết một bình luận của mình
		Task<CommentDetailResponse> GetMyCommentDetailAsync(int commentId, int userId);

		// Admin xem danh sách tất cả bình luận, có tìm kiếm và phân trang
		Task<PagedResult<CommentResponse>> GetAllCommentsAsync(CommentSearchRequest request);

		// Admin xem chi tiết một bình luận
		Task<CommentDetailResponse> GetCommentDetailAsync(int commentId);

		// Admin cập nhật bình luận
		Task<CommentDetailResponse> AdminUpdateAsync(UpdateCommentRequest request, int userId);

		// Admin xóa bình luận
		Task<CommentDetailResponse> AdminDeleteAsync(int commentId, int userId);

		// User like bình luận
		Task<int> LikeCommentAsync(int commentId, int userId);

		// User bỏ like bình luận
		Task<int> UnlikeCommentAsync(int commentId, int userId);
	}
}
