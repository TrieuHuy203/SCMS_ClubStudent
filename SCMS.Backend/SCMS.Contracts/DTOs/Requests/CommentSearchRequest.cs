namespace SCMS.Contracts.DTOs.Search;


public class CommentSearchRequest
{
	/// <summary>
	/// Trang hiện tại (mặc định 1)
	/// </summary>
	public int Page { get; set; } = 1;

	/// <summary>
	/// Số bình luận trên một trang (mặc định 10)
	/// </summary>
	public int PageSize { get; set; } = 10;

	/// <summary>
	/// Tìm kiếm theo từ khóa trong nội dung bình luận
	/// </summary>
	public string? Keyword { get; set; }

	/// <summary>
	/// Lọc theo PostId (dùng cho user xem bình luận của bài post nào)
	/// </summary>
	public int? PostId { get; set; }

	/// <summary>
	/// Lọc theo ClubId (dùng cho admin hoặc leader xem bình luận của CLB)
	/// </summary>
	public int? ClubId { get; set; }

	/// <summary>
	/// Lọc theo UserId (dùng cho admin xem bình luận của user nào)
	/// </summary>
	public int? UserId { get; set; }
}
