using System.Collections.Generic;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.DTOs.Responses;

namespace SCMS.Contracts.Interfaces.iService
{
    /// <summary>
    /// Interface định nghĩa các phương thức nghiệp vụ cho Permission.
    /// </summary>
    public interface IPermissionService
    {
        /// <summary>
        /// Lấy danh sách tất cả quyền.
        /// </summary>
        Task<IEnumerable<PermissionResponse>> GetAllAsync();

        /// <summary>
        /// Lấy quyền theo Id.
        /// </summary>
        Task<PermissionResponse> GetByIdAsync(int id);

        /// <summary>
        /// Thêm mới quyền.
        /// </summary>
        Task<PermissionResponse> CreateAsync(PermissionCreateRequest request);

        /// <summary>
        /// Cập nhật quyền.
        /// </summary>
        Task<bool> UpdateAsync(int id, PermissionUpdateRequest request);

        /// <summary>
        /// Xóa quyền theo Id.
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Tìm kiếm quyền theo tên.
        /// </summary>
        Task<IEnumerable<PermissionResponse>> SearchByNameAsync(string name);
        Task<bool> HasPermissionAsync(int userId, string permission); // Kiểm tra xem người dùng có quyền hay không
    }
}