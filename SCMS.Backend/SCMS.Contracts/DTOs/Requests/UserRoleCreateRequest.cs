namespace SCMS.Contracts.DTOs.Requests
{
    /// <summary>
    /// DTO dùng để gán vai trò cho user.
    /// </summary>
    public class UserRoleCreateRequest
    {
        /// <summary>
        /// Id của user.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Id của vai trò.
        /// </summary>
        public int RoleId { get; set; }
    }
}