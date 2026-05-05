// DTO yêu cầu tìm kiếm/phân trang danh sách báo cáo vi phạm bài viết
namespace SCMS.Contracts.DTOs.Requests
{
    public class PostReportSearchRequest
    {
        // Trang hiện tại (mặc định 1)
        public int Page { get; set; } = 1;

        // Số bản ghi trên mỗi trang (mặc định 10)
        public int PageSize { get; set; } = 10;

        // Lọc theo trạng thái report (Pending, Reviewed, Resolved, ...)
        public string? Status { get; set; }

        // Lọc theo bài viết cụ thể (nếu cần)
        public int? PostId { get; set; }

        // Lọc theo user gửi report (nếu cần)
        public int? UserId { get; set; }
    }
}