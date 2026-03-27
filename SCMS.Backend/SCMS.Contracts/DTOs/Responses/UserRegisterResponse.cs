namespace SCMS.Contracts.DTOs.Responses
{
    public class UserRegisterResponse
    {
        public bool Success { get; set; }
        public string ?Message { get; set; }
        public int UserId { get; set; } // Có thể trả về Id user vừa tạo nếu thành công
    }
}