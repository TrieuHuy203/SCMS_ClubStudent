namespace SCMS.Contracts.DTOs.Requests
{
    /// <summary>
    /// DTO dùng để gán quyền cho vai trò.
    /// </summary>
    public class RolePermissionCreateRequest
    {
        /// <summary>
        /// Id của vai trò.
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Id của quyền.
        /// </summary>
        public int PermissionId { get; set; }
    }
}