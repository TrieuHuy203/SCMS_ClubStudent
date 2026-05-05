using SCMS.DomainEntities.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCMS.Contracts.Interfaces.iRepositores
{
    public interface IRoleRepository
    {
        Task<List<Role>> GetAllRolesAsync();
        Task<Role?> GetByNameAsync(string roleName);
    }
}