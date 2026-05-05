namespace SCMS.Contracts.DTOs.Responses
{
    public class UserCreateResponse
    {
        // ID người dùng
        public int UserId { get; set; } 
        // Tên đăng nhập
        public string ?Username { get; set; } 
        // Họ và tên
        public string ?FullName { get; set; } 
        // Email    
        public string ?Email { get; set; } 
        // Số điện thoại
        public string ?Phone { get; set; } 
        // Trạng thái tài khoản
        public string ?Status { get; set; } 
        // Thời gian tạo tài khoản
        public DateTime CreatedAt { get; set; }     
        // Trạng thái vô hiệu hóa (true nếu tài khoản bị vô hiệu hóa, false nếu tài khoản đang hoạt động)
         public bool IsDisabled { get; set; } 

         // Thông điệp phản hồi (có thể dùng để truyền lỗi hoặc thông tin khác)
         public string ?Message { get; set; }
    }
}