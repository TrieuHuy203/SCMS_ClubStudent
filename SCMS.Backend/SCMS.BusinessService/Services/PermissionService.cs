using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.DTOs.Responses;
using SCMS.Contracts.Interfaces.iRepositores;
using SCMS.Contracts.Interfaces.iService;
using SCMS.DomainEntities.Entities;
// using Db context;
using SCMS.DAL;
using Microsoft.EntityFrameworkCore;

namespace SCMS.BusinessService.Services
{
    /// <summary>
    /// Triển khai các phương thức nghiệp vụ cho Permission.
    /// </summary>
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly SCMSDbContext _context;

        /// <summary>
        /// Inject repository vào service.
        /// </summary>
        public PermissionService(IPermissionRepository permissionRepository,SCMSDbContext context)
        {
            _permissionRepository = permissionRepository;
            _context = context;
        }

        /// <summary>
        /// Lấy tất cả quyền.
        /// </summary>
        public async Task<IEnumerable<PermissionResponse>> GetAllAsync()
        {
            var permissions = await _permissionRepository.GetAllAsync();
            // Chuyển đổi sang DTO response
            return permissions.Select(p => new PermissionResponse
            {
                PermissionId = p.PermissionId,
                PermissionName = p.PermissionName,
                Description = p.Description,
                IsActive = p.IsActive
                
            });
        }

        /// <summary>
        /// Lấy quyền theo Id.
        /// </summary>
        public async Task<PermissionResponse> GetByIdAsync(int id)
        {
            var p = await _permissionRepository.GetByIdAsync(id);
            if (p == null) return null;
            return new PermissionResponse
            {
                PermissionId = p.PermissionId,
                PermissionName = p.PermissionName,
                Description = p.Description,
                IsActive = p.IsActive
            };
        }

        /// <summary>
        /// Thêm mới quyền.
        /// </summary>
        public async Task<PermissionResponse> CreateAsync(PermissionCreateRequest request)
        {
            var permission = new Permission
            {
                PermissionName = request.PermissionName,
                Description = request.Description,
                // Nếu IsActive là null thì mặc định true
                IsActive = request.IsActive ?? true
            };
            await _permissionRepository.AddAsync(permission);
            return new PermissionResponse
            {
                PermissionId = permission.PermissionId,
                PermissionName = permission.PermissionName,
                Description = permission.Description,
                IsActive = permission.IsActive
            };
        }

        /// <summary>
        /// Cập nhật quyền.
        /// </summary>
        public async Task<bool> UpdateAsync(int id, PermissionUpdateRequest request)
        {
            var permission = await _permissionRepository.GetByIdAsync(id);
            if (permission == null) return false;
            permission.PermissionName = request.PermissionName;
            permission.Description = request.Description;
            // Nếu IsActive là null thì mặc định true
            permission.IsActive = request.IsActive ?? true;
            await _permissionRepository.UpdateAsync(permission);
            return true;
        }

        /// <summary>
        /// Xóa quyền theo Id.
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            var permission = await _permissionRepository.GetByIdAsync(id);
            if (permission == null) return false;
            await _permissionRepository.DeleteAsync(id);
            return true;
        }

        /// <summary>
        /// Tìm kiếm quyền theo tên.
        /// </summary>
        public async Task<IEnumerable<PermissionResponse>> SearchByNameAsync(string name)
        {
            var permissions = await _permissionRepository.SearchByNameAsync(name);
            return permissions.Select(p => new PermissionResponse
            {
                PermissionId = p.PermissionId,
                PermissionName = p.PermissionName,
                Description = p.Description,
                IsActive = p.IsActive
            });
        }
    
   




   // check quyèn trong DB RolePermission

  public async Task<bool> HasPermissionAsync(int userId, string permission)
{
    var hasPermission = await (
    from ur in _context.UserRoles
    join rp in _context.RolePermissions on ur.RoleId equals rp.RoleId
    join p in _context.Permissions on rp.PermissionId equals p.PermissionId
    where ur.UserId == userId
          && p.PermissionName == permission
    select p
).AnyAsync();

    return hasPermission;
}
}
    
    }
