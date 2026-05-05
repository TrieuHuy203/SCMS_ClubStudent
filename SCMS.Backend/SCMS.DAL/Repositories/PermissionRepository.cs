using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SCMS.Contracts.Interfaces.iRepositores;
using SCMS.DomainEntities.Entities;

namespace SCMS.DAL.Repositories
{
    /// <summary>
    /// Triển khai các phương thức thao tác với bảng Permission.
    /// </summary>
    public class PermissionRepository : IPermissionRepository
    {
        private readonly SCMSDbContext _context;

        /// <summary>
        /// Inject DbContext vào repository.
        /// </summary>
        public PermissionRepository(SCMSDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy tất cả quyền.
        /// </summary>
        public async Task<IEnumerable<Permission>> GetAllAsync()
        {
            return await _context.Permissions.ToListAsync();
        }

        /// <summary>
        /// Lấy quyền theo Id.
        /// </summary>
        public async Task<Permission> GetByIdAsync(int id)
        {
            return await _context.Permissions.FindAsync(id);
        }

        /// <summary>
        /// Thêm mới quyền.
        /// </summary>
        public async Task AddAsync(Permission permission)
        {
            await _context.Permissions.AddAsync(permission);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Cập nhật quyền.
        /// </summary>
        public async Task UpdateAsync(Permission permission)
        {
            _context.Permissions.Update(permission);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Xóa quyền theo Id.
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            var permission = await _context.Permissions.FindAsync(id);
            if (permission != null)
            {
                _context.Permissions.Remove(permission);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Tìm kiếm quyền theo tên.
        /// </summary>
        public async Task<IEnumerable<Permission>> SearchByNameAsync(string name)
        {
            return await _context.Permissions
                .Where(p => p.PermissionName.Contains(name))
                .ToListAsync();
        }
    }
}