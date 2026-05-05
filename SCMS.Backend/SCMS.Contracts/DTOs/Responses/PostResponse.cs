namespace SCMS.Contracts.DTOs.Responses
{
	/// <summary>
	/// DTO trả về thông tin bài viết ở màn danh sách.
	/// </summary>
	public class PostResponse
	{
		public int PostId { get; set; } // Khóa chính bài viết
		public string Title { get; set; } = null!; // Tiêu đề bài viết
		public string Content { get; set; } = null!; // Nội dung bài viết
		public int? ClubId { get; set; } // Id CLB
		public string? ClubName { get; set; } // Tên CLB
		public int? EventId { get; set; } // Id sự kiện
		public string? EventName { get; set; } // Tên sự kiện
		public int UserId { get; set; } // Id người đăng
		public string? FullName { get; set; } // Họ tên người đăng
		public string? PostType { get; set; } // Loại bài viết
		public string? Status { get; set; } // Trạng thái bài viết
		public string? RejectReason { get; set; } // Lý do từ chối (nếu có)
		public DateTime? CreatedAt { get; set; } // Thời điểm tạo
	}
}
