namespace SCMS.Contracts.DTOs.Requests
{
    public class ClubUpdateRequest
    {
        public int ClubId { get; set; }                  // Id của club cần cập nhật
        public string ClubName { get; set; } = null!;    // Tên mới
        public string? Description { get; set; }
        public string? Field { get; set; }
        public string? School { get; set; }
        public int? MemberCount { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        // ... các trường khác nếu cần
    }
}