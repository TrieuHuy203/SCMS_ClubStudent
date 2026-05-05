namespace SCMS.Contracts.DTOs.Responses
{
    public class UserAvatarDto
    {
        public int UserId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? AvatarUrl { get; set; }
    }
}