using System.Collections.Generic;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.DTOs.Responses;

namespace SCMS.Contracts.Interfaces.iService
{
    /// <summary>
    /// Interface nghiệp vụ cho UserPermission.
    /// </summary>
    public interface IUserPermissionService
    {
        Task<IEnumerable<UserPermissionResponse>> GetAllAsync();
        Task AddAsync(UserPermissionCreateRequest request);
        Task DeleteAsync(int userPermissionId);
        Task<IEnumerable<UserPermissionResponse>> SearchAsync(int? userId, int? permissionId);
        Task UpdateAsync(UserPermissionUpdateRequest request);
    }
}