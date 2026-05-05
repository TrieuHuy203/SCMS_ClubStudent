namespace SCMS.Contracts.DTOs.Requests
{
	/// <summary>
	/// DTO yêu cầu tạo bài viết mới.
	/// </summary>
	public class CreatePostRequest
	{
		public string Title { get; set; } = null!; // Tiêu đề bài viết
		public string Content { get; set; } = null!; // Nội dung bài viết
		public int? ClubId { get; set; } // Bài viết gắn với CLB nào (nếu có)
		public int? EventId { get; set; } // Bài viết gắn với sự kiện nào (nếu có)
		public string? PostType { get; set; } // Loại bài viết (news, announcement, ...)
	}
}
