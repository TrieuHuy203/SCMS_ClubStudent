using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.DTOs.Responses;
using SCMS.Contracts.Interfaces.iRepositores;
using SCMS.Contracts.Interfaces.iService;
using SCMS.DomainEntities.Entities;

namespace SCMS.BusinessService.Services
{
    /// <summary>
    /// Triển khai nghiệp vụ cho RolePermission.
    /// </summary>
    public class RolePermissionService : IRolePermissionService
    {
        private readonly IRolePermissionRepository _rolePermissionRepository;

        public RolePermissionService(IRolePermissionRepository rolePermissionRepository)
        {
            _rolePermissionRepository = rolePermissionRepository;
        }

        public async Task<IEnumerable<RolePermissionResponse>> GetAllAsync()
        {
            var list = await _rolePermissionRepository.GetAllAsync();
            return list.Select(rp => new RolePermissionResponse
            {
                RoleId = rp.RoleId,
                PermissionId = rp.PermissionId,
                // Có thể bổ sung RoleName, PermissionName nếu cần join
            });
        }

        public async Task AddAsync(RolePermissionCreateRequest request)
        {
            var entity = new RolePermission
            {
                RoleId = request.RoleId,
                PermissionId = request.PermissionId
            };
            await _rolePermissionRepository.AddAsync(entity);
        }

        public async Task DeleteAsync(int roleId, int permissionId)
        {
            await _rolePermissionRepository.DeleteAsync(roleId, permissionId);
        }

        public async Task<IEnumerable<RolePermissionResponse>> SearchAsync(int? roleId, int? permissionId)
        {
            var list = await _rolePermissionRepository.SearchAsync(roleId, permissionId);
            return list.Select(rp => new RolePermissionResponse
            {
                RoleId = rp.RoleId,
                PermissionId = rp.PermissionId
            });
        }
    }
}