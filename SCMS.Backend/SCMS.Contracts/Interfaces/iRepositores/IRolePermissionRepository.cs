using System.Collections.Generic;
using System.Threading.Tasks;
using SCMS.DomainEntities.Entities;

namespace SCMS.Contracts.Interfaces.iRepositores
{
    /// <summary>
    /// Interface thao tác với bảng RolePermission.
    /// </summary>
    public interface IRolePermissionRepository
    {
        /// <summary>
        /// Lấy tất cả RolePermission.
        /// </summary>
        Task<IEnumerable<RolePermission>> GetAllAsync();

        /// <summary>
        /// Thêm mới RolePermission.
        /// </summary>
        Task AddAsync(RolePermission rolePermission);

        /// <summary>
        /// Xóa RolePermission theo RoleId và PermissionId.
        /// </summary>
        Task DeleteAsync(int roleId, int permissionId);

        /// <summary>
        /// Tìm kiếm RolePermission theo RoleId hoặc PermissionId.
        /// </summary>
        Task<IEnumerable<RolePermission>> SearchAsync(int? roleId, int? permissionId);
    }
}