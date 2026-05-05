// DTO trả về thông tin báo cáo vi phạm bài viết cho admin
namespace SCMS.Contracts.DTOs.Responses
{
    public class PostReportResponse
    {
        public int PostReportId { get; set; } // Khóa chính report
        public int PostId { get; set; } // Id bài viết bị báo cáo
        public int UserId { get; set; } // Id user gửi report
        public string Reason { get; set; } = null!; // Lý do báo cáo
        public string? Description { get; set; } // Mô tả chi tiết
        public string? Status { get; set; } // Trạng thái xử lý (Pending, Reviewed, Resolved...)
        public DateTime? CreatedAt { get; set; } // Thời điểm gửi report

        // (Có thể bổ sung thêm các trường liên quan đến bài viết hoặc user nếu cần)
    }
}