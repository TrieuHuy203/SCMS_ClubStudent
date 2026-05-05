namespace SCMS.Contracts.DTOs.Requests
{
    public class AdminMembershipListRequestDto
    {
        public int? ClubId { get; set; }
        public string? UserName { get; set; }
        public string? Status { get; set; } // "Pending", "Approved", "Rejected"
        public int Page { get; set; } = 1;

    public int? UserId { get; set; } // thêm dòng này
        public int PageSize { get; set; } = 10;
        // public int? UserId { get; set; }
    }
}