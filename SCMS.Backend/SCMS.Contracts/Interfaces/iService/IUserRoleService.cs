using System.Collections.Generic;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.DTOs.Responses;

namespace SCMS.Contracts.Interfaces.iService
{
    /// <summary>
    /// Interface nghiệp vụ cho UserRole.
    /// </summary>
    public interface IUserRoleService
    {
        Task<IEnumerable<UserRoleResponse>> GetAllAsync();
        Task AddAsync(UserRoleCreateRequest request);
        Task DeleteAsync(int userRoleId);
        Task<IEnumerable<UserRoleResponse>> SearchAsync(int? userId, int? roleId);
    }
}