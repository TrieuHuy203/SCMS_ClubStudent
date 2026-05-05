namespace SCMS.Contracts.DTOs.Responses
{
    public class ClubMemberResponse
    {
        public int MembershipId { get; set; }
        public int UserId { get; set; }
        public string? FullName { get; set; }
        public string? Username { get; set; }
        public string? Role { get; set; }
        public string? Status { get; set; }
        public DateTime? JoinedAt { get; set; }
    }
}
