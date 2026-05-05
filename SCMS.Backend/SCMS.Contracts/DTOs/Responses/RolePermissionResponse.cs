namespace SCMS.Contracts.DTOs.Responses
{
    /// <summary>
    /// DTO trả về thông tin RolePermission.
    /// </summary>
    public class RolePermissionResponse
    {
        /// <summary>
        /// Id của vai trò.
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Id của quyền.
        /// </summary>
        public int PermissionId { get; set; }

        /// <summary>
        /// Tên vai trò (nếu cần).
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// Tên quyền (nếu cần).
        /// </summary>
        public string PermissionName { get; set; }
    }
}