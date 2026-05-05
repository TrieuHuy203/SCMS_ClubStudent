namespace SCMS.Contracts.DTOs.Requests
{
	/// <summary>
	/// DTO yêu cầu tìm kiếm và phân trang bài viết.
	/// </summary>
	public class PostSearchRequest
	{
		public int Page { get; set; } = 1; // Trang hiện tại
		public int PageSize { get; set; } = 10; // Số bản ghi trên mỗi trang
		public string? Keyword { get; set; } // Từ khóa tìm theo tiêu đề, nội dung
		public string? Status { get; set; } // Lọc theo trạng thái bài viết
		public string? PostType { get; set; } // Lọc theo loại bài viết
		public int? ClubId { get; set; } // Lọc theo CLB
		public int? EventId { get; set; } // Lọc theo sự kiện
	}
}
