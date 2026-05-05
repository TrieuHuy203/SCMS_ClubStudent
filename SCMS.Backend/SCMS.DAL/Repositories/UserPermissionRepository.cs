using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SCMS.Contracts.Interfaces.iRepositores;
using SCMS.DomainEntities.Entities;

namespace SCMS.DAL.Repositories
{
    /// <summary>
    /// Triển khai thao tác với bảng UserPermissions.
    /// </summary>
    public class UserPermissionRepository : IUserPermissionRepository
    {
        private readonly SCMSDbContext _context;

        public UserPermissionRepository(SCMSDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserPermission>> GetAllAsync()
        {
            return await _context.UserPermissions.ToListAsync();
        }

        public async Task AddAsync(UserPermission userPermission)
        {
            await _context.UserPermissions.AddAsync(userPermission);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int userPermissionId)
        {
            var entity = await _context.UserPermissions.FindAsync(userPermissionId);
            if (entity != null)
            {
                _context.UserPermissions.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<UserPermission>> SearchAsync(int? userId, int? permissionId)
        {
            var query = _context.UserPermissions.AsQueryable();
            if (userId.HasValue)
                query = query.Where(up => up.UserId == userId.Value);
            if (permissionId.HasValue)
                query = query.Where(up => up.PermissionId == permissionId.Value);
            return await query.ToListAsync();
        }
         public async Task<UserPermission> GetByIdAsync(int userPermissionId)
            {
                return await _context.UserPermissions.FindAsync(userPermissionId);
            }

            public async Task UpdateAsync(UserPermission userPermission)
            {
                _context.UserPermissions.Update(userPermission);
                await _context.SaveChangesAsync();
            }
    }
}