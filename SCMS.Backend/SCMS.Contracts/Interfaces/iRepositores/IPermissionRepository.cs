using System.Collections.Generic;
using System.Threading.Tasks;
using SCMS.DomainEntities.Entities;

namespace SCMS.Contracts.Interfaces.iRepositores
{
    /// <summary>
    /// Interface định nghĩa các phương thức thao tác với bảng Permission.
    /// </summary>
    public interface IPermissionRepository
    {
        /// <summary>
        /// Lấy danh sách tất cả quyền.
        /// </summary>
        Task<IEnumerable<Permission>> GetAllAsync();

        /// <summary>
        /// Lấy quyền theo Id.
        /// </summary>
        Task<Permission> GetByIdAsync(int id);

        /// <summary>
        /// Thêm mới quyền.
        /// </summary>
        Task AddAsync(Permission permission);

        /// <summary>
        /// Cập nhật quyền.
        /// </summary>
        Task UpdateAsync(Permission permission);

        /// <summary>
        /// Xóa quyền theo Id.
        /// </summary>
        Task DeleteAsync(int id);

        /// <summary>
        /// Tìm kiếm quyền theo tên.
        /// </summary>
        Task<IEnumerable<Permission>> SearchByNameAsync(string name);
    }
}