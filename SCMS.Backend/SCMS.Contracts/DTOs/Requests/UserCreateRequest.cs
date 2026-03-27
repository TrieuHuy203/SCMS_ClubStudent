namespace SCMS.Contracts.DTOs.Requests
{
    public class UserCreateRequest
    {
        // Tên đăng nhập 
        public string  Username { get; set; }  
        // Mật khẩu  
        public string  Password { get; set; } 
        // Họ và tên
        public string FullName { get; set; } 
        // Email
        public string Email { get; set; } 
        // Số điện thoại
        public string Phone { get; set; } 
        // Thông điệp (có thể dùng để truyền lỗi hoặc thông tin khác)
        public string ?Message { get; set; }
    }
}