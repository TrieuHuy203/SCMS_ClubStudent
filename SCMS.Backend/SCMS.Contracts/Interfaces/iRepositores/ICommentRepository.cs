using System.Collections.Generic;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Search;
using SCMS.DomainEntities.Entities;

namespace SCMS.Contracts.Interfaces.iRepositores
{
	// Interface thao tác dữ liệu bình luận
	public interface ICommentRepository
	{
		// Tạo mới bình luận
		Task<Comment> AddAsync(Comment comment);

		// Cập nhật bình luận
		Task<Comment> UpdateAsync(Comment comment);

		// Xóa bình luận (hard delete)
		Task DeleteAsync(Comment comment);

		// Lấy bình luận theo id
		Task<Comment?> GetByIdAsync(int commentId);

		// Lấy bình luận theo id và userId để kiểm tra quyền sở hữu
		Task<Comment?> GetByIdAsync(int commentId, int userId);

		// Lấy bình luận dạng tracked theo id để cập nhật/xóa (admin)
		Task<Comment?> GetByIdForUpdateAsync(int commentId);

		// Lấy bình luận dạng tracked theo id + userId để cập nhật/xóa (user)
		Task<Comment?> GetByIdForUpdateAsync(int commentId, int userId);

		// Lấy danh sách bình luận của user, có tìm kiếm và phân trang
		Task<(List<Comment> Items, int TotalCount)> GetMyCommentsAsync(
			int userId,
			CommentSearchRequest request);

		// Lấy danh sách tất cả bình luận, có tìm kiếm và phân trang cho admin
		Task<(List<Comment> Items, int TotalCount)> GetAllCommentsAsync(CommentSearchRequest request);

		// Lấy like của một user trên một bình luận cụ thể
		Task<Like?> GetCommentLikeAsync(int commentId, int userId);

		// Thêm like cho bình luận
		Task<Like> AddCommentLikeAsync(Like like);

		// Xóa like khỏi bình luận
		Task RemoveCommentLikeAsync(Like like);

		// Đếm tổng like của bình luận
		Task<int> CountCommentLikesAsync(int commentId);
	}
}
