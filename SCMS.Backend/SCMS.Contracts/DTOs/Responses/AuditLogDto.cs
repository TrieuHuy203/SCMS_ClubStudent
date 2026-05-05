namespace SCMS.Contracts.DTOs.Responses
{
    public class AuditLogDto
    {
        public int AuditLogId { get; set; }
        public int UserId { get; set; }
        public string? TableName { get; set; }
        public int RecordId { get; set; }
        public string? ActionType { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public DateTime? ActionTime { get; set; }
        // Có thể bổ sung thêm UserName nếu cần join với bảng User
        public string? UserName { get; set; }
    }
}