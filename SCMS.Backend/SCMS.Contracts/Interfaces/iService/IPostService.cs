using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.DTOs.Responses;

namespace SCMS.Contracts.Interfaces.iService
{
	// Interface nghiệp vụ cho bài viết
	public interface IPostService
	{
		// User tạo bài viết mới
		Task<PostDetailResponse> CreateAsync(CreatePostRequest request, int userId);

		// User cập nhật bài viết của chính mình
		Task<PostDetailResponse> UpdateAsync(UpdatePostRequest request, int userId);

		// User xóa bài viết của chính mình
		Task<PostDetailResponse> DeleteAsync(int postId, int userId);

		// User xem danh sách bài viết của mình, có tìm kiếm và phân trang
		Task<PagedResult<PostResponse>> GetMyPostsAsync(int userId, PostSearchRequest request);

		// User xem danh sách bài viết của các CLB mình đã tham gia
		Task<PagedResult<PostResponse>> GetMyClubPostsAsync(int userId, PostSearchRequest request);

		// User xem danh sách bài viết công khai của toàn hệ thống
		Task<PagedResult<PostResponse>> GetPublicPostsAsync(PostSearchRequest request);

		// User xem chi tiết một bài viết của mình
		Task<PostDetailResponse> GetMyPostDetailAsync(int postId, int userId);

		// Admin xem danh sách tất cả bài viết, có tìm kiếm và phân trang
		Task<PagedResult<PostResponse>> GetAllPostsAsync(PostSearchRequest request);

		// Admin xem chi tiết một bài viết
		Task<PostDetailResponse> GetPostDetailAsync(int postId);

		// Admin tạo bài viết mới
		Task<PostDetailResponse> AdminCreateAsync(CreatePostRequest request, int userId);

		// Admin cập nhật bài viết
		Task<PostDetailResponse> AdminUpdateAsync(UpdatePostRequest request, int userId);

		// Admin xoá bài viết
		Task<PostDetailResponse> AdminDeleteAsync(int postId, int userId);

		// Admin duyệt bài viết
		Task<PostDetailResponse> ApproveAsync(int postId, int adminId);

		// Admin từ chối bài viết
		Task<PostDetailResponse> RejectAsync(int postId, int adminId, string rejectReason);

		// User like bài viết
		Task<int> LikePostAsync(int postId, int userId);

		// User bỏ like bài viết
		Task<int> UnlikePostAsync(int postId, int userId);

		// User báo cáo bài viết
		Task<string> ReportPostAsync(int postId, int userId, CreatePostReportRequest request);
	// Thêm vào IPostService.cs
Task<PagedResult<PostResponse>> GetLikedPostsAsync(int userId, PostSearchRequest request);
// object có thể thay bằng kiểu trả về thực tế của bạn (ví dụ: PagedResult<PostResponse>)
	}
}
