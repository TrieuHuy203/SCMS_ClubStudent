namespace SCMS.Contracts.DTOs.Requests
{
    /// <summary>
    /// DTO dùng để gán quyền cho user.
    /// </summary>
    public class UserPermissionCreateRequest
    {
        /// <summary>
        /// Id của user.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Id của quyền.
        /// </summary>
        public int PermissionId { get; set; }
    }
}