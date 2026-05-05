namespace SCMS.Contracts.DTOs.Requests
{
    // DTO user gửi khi báo cáo bài viết
    public class CreatePostReportRequest
    {
        public string Reason { get; set; } = string.Empty; // Lý do báo cáo (bắt buộc)
        public string? Description { get; set; } // Mô tả chi tiết (không bắt buộc)
    }
}
