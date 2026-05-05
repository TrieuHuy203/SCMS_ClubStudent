using SCMS.Contracts.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCMS.Contracts.Interfaces.iService
{
    public interface IRoleService
    {
        Task<List<RoleDto>> GetAllRolesAsync();
    }
}