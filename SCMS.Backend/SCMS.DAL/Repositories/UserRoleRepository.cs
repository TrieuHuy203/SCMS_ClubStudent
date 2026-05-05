using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SCMS.Contracts.Interfaces.iRepositores;
using SCMS.DomainEntities.Entities;

namespace SCMS.DAL.Repositories
{
    /// <summary>
    /// Triển khai thao tác với bảng UserRoles.
    /// </summary>
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly SCMSDbContext _context;

        public UserRoleRepository(SCMSDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserRole>> GetAllAsync()
        {
            return await _context.UserRoles.ToListAsync();
        }

        public async Task AddAsync(UserRole userRole)
        {
            await _context.UserRoles.AddAsync(userRole);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int userRoleId)
        {
            var entity = await _context.UserRoles.FindAsync(userRoleId);
            if (entity != null)
            {
                _context.UserRoles.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<UserRole>> SearchAsync(int? userId, int? roleId)
        {
            var query = _context.UserRoles.AsQueryable();
            if (userId.HasValue)
                query = query.Where(ur => ur.UserId == userId.Value);
            if (roleId.HasValue)
                query = query.Where(ur => ur.RoleId == roleId.Value);
            return await query.ToListAsync();
        }
    }
}