namespace SCMS.Contracts.DTOs.Requests
{
    public class UserRegisterRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string? Message { get; set; }
        // Thêm các trường khác nếu cần (ví dụ: Role, Address, ...)
    }
}