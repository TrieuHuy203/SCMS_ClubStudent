namespace SCMS.Contracts.DTOs.Responses;

public class CommentResponse
{
	/// <summary>
	/// ID của bình luận
	/// </summary>
	public int CommentId { get; set; }

	/// <summary>
	/// ID của bài viết
	/// </summary>
	public int PostId { get; set; }

	/// <summary>
	/// ID của người bình luận
	/// </summary>
	public int UserId { get; set; }

	/// <summary>
	/// Tên người bình luận
	/// </summary>
	public string? FullName { get; set; }

	/// <summary>
	/// Email của người bình luận
	/// </summary>
	public string? Email { get; set; }

	/// <summary>
	/// Nội dung bình luận
	/// </summary>
	public string Content { get; set; } = null!;

	/// <summary>
	/// Ngày tạo bình luận
	/// </summary>
	public DateTime? CreatedAt { get; set; }

	/// <summary>
	/// Số lương like của bình luận
	/// </summary>
	public int LikeCount { get; set; }
}
