// DTO trả về kết quả sau khi sửa user
namespace SCMS.Contracts.DTOs.Responses
{
    public class UserUpdateResponse
    {
        // ID người dùng cần sửa
        public int UserId { get; set; }
        // Tên đăng nhập (không cho phép sửa, chỉ để hiển thị)
        public string ?Username { get; set; }
        // Họ tên mới
        public string ?FullName { get; set; }
        // Email mới
        public string ?Email { get; set; }
        // Số điện thoại mới
        public string? Phone { get; set; }
        // Trạng thái tài khoản mới
        public string? Status { get; set; }
        //  Thông điệp phản hồi (có thể dùng để truyền lỗi hoặc thông tin khác)
        public string ?Message { get; set; }
    }
}