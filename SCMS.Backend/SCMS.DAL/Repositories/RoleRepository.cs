using SCMS.Contracts.Interfaces.iRepositores;
using SCMS.DomainEntities.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCMS.DAL.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly SCMSDbContext _context;

        public RoleRepository(SCMSDbContext context)
        {
            _context = context;
        }

        public async Task<List<Role>> GetAllRolesAsync()
        {
            return await _context.Roles.AsNoTracking().ToListAsync();
        }
         public async Task<Role?> GetByNameAsync(string roleName)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(r => r.RoleName == roleName);
    }
    }
}