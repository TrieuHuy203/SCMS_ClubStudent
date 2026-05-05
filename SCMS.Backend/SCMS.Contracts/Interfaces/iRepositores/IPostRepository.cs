using System.Collections.Generic;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.DomainEntities.Entities;

namespace SCMS.Contracts.Interfaces.iRepositores
{
	// Interface thao tác dữ liệu bài viết
	public interface IPostRepository
	{
		// Tạo mới bài viết
		Task<Post> AddAsync(Post post);

		// Cập nhật bài viết
		Task<Post> UpdateAsync(Post post);

		// Xóa bài viết
		Task DeleteAsync(Post post);

		// Lấy bài viết theo id
		Task<Post?> GetByIdAsync(int postId);

		// Lấy bài viết theo id và userId để kiểm tra quyền sở hữu
		Task<Post?> GetByIdAsync(int postId, int userId);

		// Lấy bài viết dạng tracked theo id để cập nhật/xóa (admin)
		Task<Post?> GetByIdForUpdateAsync(int postId);

		// Lấy bài viết dạng tracked theo id + userId để cập nhật/xóa (user)
		Task<Post?> GetByIdForUpdateAsync(int postId, int userId);

		// Lấy danh sách bài viết của user, có tìm kiếm và phân trang
		Task<(List<Post> Items, int TotalCount)> GetMyPostsAsync(
			int userId,
			PostSearchRequest request);

		// Lấy danh sách bài viết của các CLB mà user đã tham gia, có tìm kiếm và phân trang
		Task<(List<Post> Items, int TotalCount)> GetMyClubPostsAsync(
			IEnumerable<int> clubIds,
			PostSearchRequest request);

		// Lấy danh sách bài viết công khai toàn hệ thống (Approved), có tìm kiếm và phân trang
		Task<(List<Post> Items, int TotalCount)> GetPublicPostsAsync(PostSearchRequest request);

		// Lấy danh sách tất cả bài viết, có tìm kiếm và phân trang cho admin
		Task<(List<Post> Items, int TotalCount)> GetAllPostsAsync(PostSearchRequest request);

		// Kiểm tra user đã like bài viết hay chưa
		Task<Like?> GetPostLikeAsync(int postId, int userId);

		// Thêm like cho bài viết
		Task<Like> AddPostLikeAsync(Like like);

		// Bỏ like bài viết
		Task RemovePostLikeAsync(Like like);

		// Lấy tổng số lượt like của bài viết
		Task<int> CountPostLikesAsync(int postId);

		// Kiểm tra user đã report bài viết hay chưa
		Task<bool> HasUserReportedPostAsync(int postId, int userId);

		// Lưu báo cáo bài viết
		Task<PostReport> AddPostReportAsync(PostReport report);
		 Task<(List<Post> Items, int TotalCount)> GetLikedPostsAsync(int userId, PostSearchRequest request);
	}
}
