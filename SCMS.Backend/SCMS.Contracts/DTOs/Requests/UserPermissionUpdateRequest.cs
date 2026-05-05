namespace SCMS.Contracts.DTOs.Requests
{
    public class UserPermissionUpdateRequest
    {
        public int UserPermissionId { get; set; }
        public int? PermissionId { get; set; }
        public int? UserId { get; set; }
        // Thêm các trường khác nếu cần
    }
}