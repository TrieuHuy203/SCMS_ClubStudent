namespace SCMS.Contracts.DTOs.Requests
{
	public class EventRegistrationSearchRequest
	{
		// Từ khóa tìm theo tên sự kiện
		public string? Keyword { get; set; }

		// Lọc theo trạng thái đăng ký: Registered, Cancelled, ...
		public string? RegistrationStatus { get; set; }

		public int Page { get; set; } = 1;
		public int PageSize { get; set; } = 10;
	}
}
