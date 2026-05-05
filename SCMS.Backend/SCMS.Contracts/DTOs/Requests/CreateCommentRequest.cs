namespace SCMS.Contracts.DTOs.Requests;

public class CreateCommentRequest
{
	/// <summary>
	/// ID của bài viết được bình luận
	/// </summary>
	public int PostId { get; set; }

	/// <summary>
	/// Nội dung bình luận
	/// </summary>
	public string Content { get; set; } = null!;
}
