namespace SCMS.Contracts.DTOs.Requests
{
	/// <summary>
	/// DTO yêu cầu từ chối bài viết.
	/// </summary>
	public class RejectPostRequest
	{
		public string RejectReason { get; set; } = null!; // Lý do từ chối
	}
}