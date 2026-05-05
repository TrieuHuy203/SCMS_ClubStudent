using System.Collections.Generic;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.DTOs.Responses;

namespace SCMS.Contracts.Interfaces.iService
{
    /// <summary>
    /// Interface nghiệp vụ cho RolePermission.
    /// </summary>
    public interface IRolePermissionService
    {
        Task<IEnumerable<RolePermissionResponse>> GetAllAsync();
        Task AddAsync(RolePermissionCreateRequest request);
        Task DeleteAsync(int roleId, int permissionId);
        Task<IEnumerable<RolePermissionResponse>> SearchAsync(int? roleId, int? permissionId);
    }
}