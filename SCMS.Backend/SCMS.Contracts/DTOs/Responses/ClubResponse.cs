namespace SCMS.Contracts.DTOs.Responses
{
    public class ClubResponse
    {
        public int ClubId { get; set; }           // Khóa chính
        public string ClubName { get; set; }      // Tên câu lạc bộ
        public string? Description { get; set; }
        public string? Field { get; set; }
        public string? School { get; set; }
        public int? MemberCount { get; set; }
        public string? Status { get; set; }
        public string? RejectReason { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? CreatedByUserId { get; set; } // Id người tạo CLB
        // ... các trường khác nếu cần
    }
}