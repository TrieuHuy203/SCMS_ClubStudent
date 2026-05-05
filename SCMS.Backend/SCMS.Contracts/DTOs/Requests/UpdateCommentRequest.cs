namespace SCMS.Contracts.DTOs.Requests;

public class UpdateCommentRequest
{
	/// <summary>
	/// ID của bình luận cần sửa
	/// </summary>
	public int CommentId { get; set; }

	/// <summary>
	/// Nội dung bình luận cập nhật
	/// </summary>
	public string Content { get; set; } = null!;
}
