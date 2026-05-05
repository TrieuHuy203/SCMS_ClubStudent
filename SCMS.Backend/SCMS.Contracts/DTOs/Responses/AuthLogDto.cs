namespace SCMS.Contracts.DTOs.Responses
{
    public class AuthLogDto
    {
        public int AuthLogId { get; set; }
        public int UserId { get; set; }
        public string? ActionType { get; set; }
        public string? DeviceInfo { get; set; }
        public string? IPAddress { get; set; }
        public DateTime? ActionTime { get; set; }
        public string? Status { get; set; }
        // Có thể bổ sung thêm UserName nếu cần join với bảng User
        public string? UserName { get; set; }
    }
}