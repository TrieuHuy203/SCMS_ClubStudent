namespace SCMS.Contracts.DTOs.Requests
{
    public class ClubCreateRequest
    {
        public string ClubName { get; set; } = null!;    // Tên câu lạc bộ
        public string? Description { get; set; }
        public string? Field { get; set; }
        public string? School { get; set; }
        public int? MemberCount { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        // ... các trường khác nếu cần
    }
}