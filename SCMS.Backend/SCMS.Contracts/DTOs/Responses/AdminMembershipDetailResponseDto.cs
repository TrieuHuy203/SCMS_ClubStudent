namespace SCMS.Contracts.DTOs.Responses
{
    public class AdminMembershipDetailResponseDto
    {
        public int MembershipId { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public int ClubId { get; set; }
        public string? ClubName { get; set; }
        public string? Role { get; set; }
        public string? DesiredRole { get; set; }
        public string? Status { get; set; }
        public string? RegisterReason { get; set; }
        public string? Skills { get; set; }
        public string? Experience { get; set; }
        public DateTime? JoinedAt { get; set; }
        public DateTime? LeftAt { get; set; }
        public string? ApproveOrRejectNote { get; set; }
    }
}