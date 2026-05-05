namespace SCMS.Contracts.DTOs.Requests
{
    public class UserProfileUpdateRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? AvatarUrl { get; set; }
    }
}