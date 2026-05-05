namespace SCMS.Contracts.DTOs.Responses
{
    /// <summary>
    /// DTO trả về thông tin UserRole.
    /// </summary>
    public class UserRoleResponse
    {
        /// <summary>
        /// Id của bản ghi UserRole.
        /// </summary>
        public int UserRoleId { get; set; }

        /// <summary>
        /// Id của user.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Id của vai trò.
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Tên user (nếu cần).
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Tên vai trò (nếu cần).
        /// </summary>
        public string RoleName { get; set; }
    }
}