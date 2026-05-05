namespace SCMS.Contracts.DTOs.Requests
{
	/// <summary>
	/// DTO yêu cầu tìm kiếm và phân trang danh sách CLB đã yêu thích.
	/// </summary>
	public class ClubFavoriteSearchRequest
	{
		public int Page { get; set; } = 1; // Trang hiện tại
		public int PageSize { get; set; } = 10; // Số bản ghi trên mỗi trang
		public string? Keyword { get; set; } // Từ khóa tìm kiếm theo tên CLB hoặc mô tả
	}
}
