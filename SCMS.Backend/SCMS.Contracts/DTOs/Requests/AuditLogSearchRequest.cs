namespace SCMS.Contracts.DTOs.Requests
{
    public class AuditLogSearchRequest
    {
        public int? UserId { get; set; }
        public string? TableName { get; set; }
        public int? RecordId { get; set; }
        public string? ActionType { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}