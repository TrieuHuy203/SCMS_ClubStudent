namespace SCMS.Contracts.DTOs.Responses
{
    /// <summary>
    /// DTO trả về thông tin UserPermission.
    /// </summary>
    public class UserPermissionResponse
    {
        /// <summary>
        /// Id của bản ghi UserPermission.
        /// </summary>
        public int UserPermissionId { get; set; }

        /// <summary>
        /// Id của user.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Id của quyền.
        /// </summary>
        public int PermissionId { get; set; }

        /// <summary>
        /// Tên user (nếu cần).
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Tên quyền (nếu cần).
        /// </summary>
        public string PermissionName { get; set; }
    }
}