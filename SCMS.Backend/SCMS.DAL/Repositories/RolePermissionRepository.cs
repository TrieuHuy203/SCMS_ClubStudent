using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SCMS.Contracts.Interfaces.iRepositores;
using SCMS.DomainEntities.Entities;

namespace SCMS.DAL.Repositories
{
    /// <summary>
    /// Triển khai thao tác với bảng RolePermission.
    /// </summary>
    public class RolePermissionRepository : IRolePermissionRepository
    {
        private readonly SCMSDbContext _context;

        public RolePermissionRepository(SCMSDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RolePermission>> GetAllAsync()
        {
            return await _context.RolePermissions.ToListAsync();
        }

        public async Task AddAsync(RolePermission rolePermission)
        {
            await _context.RolePermissions.AddAsync(rolePermission);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int roleId, int permissionId)
        {
            var entity = await _context.RolePermissions
                .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);
            if (entity != null)
            {
                _context.RolePermissions.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<RolePermission>> SearchAsync(int? roleId, int? permissionId)
        {
            var query = _context.RolePermissions.AsQueryable();
            if (roleId.HasValue)
                query = query.Where(rp => rp.RoleId == roleId.Value);
            if (permissionId.HasValue)
                query = query.Where(rp => rp.PermissionId == permissionId.Value);
            return await query.ToListAsync();
        }
    }
}