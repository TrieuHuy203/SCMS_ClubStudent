// DTO nhận dữ liệu từ client khi sửa user
namespace SCMS.Contracts.DTOs.Requests
{
    public class UserUpdateRequest
    {
        // Họ tên mới (bắt buộc)
        public string ?FullName { get; set; }

        // Email mới (bắt buộc)
        public string ?Email { get; set; }

        // Số điện thoại mới (không bắt buộc)
        public string? Phone { get; set; }

        // Trạng thái tài khoản (Active/Inactive/...)
        public string? Status { get; set; }
    }
}