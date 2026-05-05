using System.Collections.Generic;
using System.Threading.Tasks;
using SCMS.DomainEntities.Entities;

namespace SCMS.Contracts.Interfaces.iRepositores
{
    /// <summary>
    /// Interface thao tác với bảng UserPermissions.
    /// </summary>
    public interface IUserPermissionRepository
    {
        /// <summary>
        /// Lấy tất cả UserPermission.
        /// </summary>
        Task<IEnumerable<UserPermission>> GetAllAsync();

        /// <summary>
        /// Thêm mới UserPermission.
        /// </summary>
        Task AddAsync(UserPermission userPermission);

        /// <summary>
        /// Xóa UserPermission theo UserPermissionId.
        /// </summary>
        Task DeleteAsync(int userPermissionId);

        /// <summary>
        /// Tìm kiếm UserPermission theo UserId hoặc PermissionId.
        /// </summary>
        Task<IEnumerable<UserPermission>> SearchAsync(int? userId, int? permissionId);

        /// <summary>
        /// Lấy UserPermission theo Id.
        /// </summary>
        Task<UserPermission> GetByIdAsync(int userPermissionId);

        /// <summary>
        /// Cập nhật UserPermission.
        /// </summary>
        Task UpdateAsync(UserPermission userPermission);
    }
}