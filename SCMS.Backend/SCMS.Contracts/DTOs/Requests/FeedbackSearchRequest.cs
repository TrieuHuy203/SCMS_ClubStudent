namespace SCMS.Contracts.DTOs.Requests
{
	/// <summary>
	/// DTO yêu cầu tìm kiếm và phân trang feedback.
	/// </summary>
	public class FeedbackSearchRequest
	{
		public int Page { get; set; } = 1; // Trang hiện tại
		public int PageSize { get; set; } = 10; // Số bản ghi trên mỗi trang
		public string? Keyword { get; set; } // Từ khóa tìm trong nội dung feedback
		public string? Status { get; set; } // Lọc theo trạng thái feedback
		public int? UserId { get; set; } // Lọc theo người gửi feedback
	}
}
