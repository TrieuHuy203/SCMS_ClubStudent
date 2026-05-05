using System.Collections.Generic;
using System.Threading.Tasks;
using SCMS.DomainEntities.Entities;

namespace SCMS.Contracts.Interfaces.iRepositores
{
    /// <summary>
    /// Interface thao tác với bảng UserRoles.
    /// </summary>
    public interface IUserRoleRepository
    {
        /// <summary>
        /// Lấy tất cả UserRole.
        /// </summary>
        Task<IEnumerable<UserRole>> GetAllAsync();

        /// <summary>
        /// Thêm mới UserRole.
        /// </summary>
        Task AddAsync(UserRole userRole);

        /// <summary>
        /// Xóa UserRole theo UserRoleId.
        /// </summary>
        Task DeleteAsync(int userRoleId);

        /// <summary>
        /// Tìm kiếm UserRole theo UserId hoặc RoleId.
        /// </summary>
        Task<IEnumerable<UserRole>> SearchAsync(int? userId, int? roleId);
    }
}