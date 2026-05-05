namespace SCMS.Contracts.DTOs.Requests
{
	/// <summary>
	/// DTO yêu cầu tạo feedback mới.
	/// </summary>
	public class CreateFeedbackRequest
	{
		public string Content { get; set; } = null!; // Nội dung góp ý từ user
	}
}
