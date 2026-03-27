// SCMS.Contracts/DTOs/Requests/ClubSearchRequest.cs
namespace SCMS.Contracts.DTOs.Requests
{
    /// <summary>
    /// DTO yêu cầu tìm kiếm & phân trang Club
    /// </summary>
    public class ClubSearchRequest
    {
        public int Page { get; set; } = 1; // Trang hiện tại
        public int PageSize { get; set; } = 10; // Số bản ghi/trang
        public string? Keyword { get; set; } // Từ khóa tìm kiếm (tên, mô tả, ...)
        public string? Status { get; set; } // Lọc theo trạng thái
        public string? Field { get; set; } // Lọc theo lĩnh vực
        public string? School { get; set; } // Lọc theo trường
        
    }
}