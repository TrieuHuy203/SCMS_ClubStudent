namespace SCMS.Contracts.DTOs.Responses
{
    public class MembershipResponse
    {
        public int MembershipId { get; set; }
        public int UserId { get; set; }
        public int ClubId { get; set; }
        public string? ClubName { get; set; }
        public string? Role { get; set; }
        public string? Status { get; set; }
        public DateTime? JoinedAt { get; set; }
        public DateTime? LeftAt { get; set; }

        // Các trường bổ sung
        public string? RegisterReason { get; set; }    // Lý do muốn tham gia CLB
        public string? Skills { get; set; }            // Kỹ năng/sở thích liên quan
        public string? Experience { get; set; }        // Kinh nghiệm tham gia CLB/hoạt động ngoại khóa
        public string? DesiredRole { get; set; }       // Vai trò mong muốn

        // Nếu muốn trả về thông tin cá nhân từ profile:
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        // ... các trường khác nếu cần
    }
}