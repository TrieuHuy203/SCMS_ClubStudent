namespace SCMS.Contracts.DTOs.Responses
{
	/// <summary>
	/// DTO trả về thông tin feedback ở màn danh sách.
	/// </summary>
	public class FeedbackResponse
	{
		public int FeedbackId { get; set; } // Khóa chính feedback
		public int UserId { get; set; } // Id user gửi feedback
		public string? FullName { get; set; } // Họ tên user gửi
		public string? Email { get; set; } // Email user gửi
		public string Content { get; set; } = null!; // Nội dung feedback
		public string Status { get; set; } = null!; // Trạng thái xử lý
		public DateTime CreatedAt { get; set; } // Thời điểm tạo
		public DateTime? ProcessedAt { get; set; } // Thời điểm admin xử lý
		public int? ProcessedBy { get; set; } // Id admin xử lý
		public string? ProcessedByName { get; set; } // Tên admin xử lý
	}
}
