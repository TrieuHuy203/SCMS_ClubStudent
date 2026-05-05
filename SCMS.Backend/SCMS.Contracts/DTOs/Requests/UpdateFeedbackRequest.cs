namespace SCMS.Contracts.DTOs.Requests
{
	/// <summary>
	/// DTO yêu cầu cập nhật feedback.
	/// </summary>
	public class UpdateFeedbackRequest
	{
		public int FeedbackId { get; set; } // Khóa chính feedback
		public string Content { get; set; } = null!; // Nội dung mới
	}
}
