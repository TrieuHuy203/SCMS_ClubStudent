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
    /// Triển khai nghiệp vụ cho UserPermission.
    /// </summary>
    public class UserPermissionService : IUserPermissionService
    {
        private readonly IUserPermissionRepository _userPermissionRepository;

        public UserPermissionService(IUserPermissionRepository userPermissionRepository)
        {
            _userPermissionRepository = userPermissionRepository;
        }

        public async Task<IEnumerable<UserPermissionResponse>> GetAllAsync()
        {
            var list = await _userPermissionRepository.GetAllAsync();
            return list.Select(up => new UserPermissionResponse
            {
                UserPermissionId = up.UserPermissionId,
                UserId = up.UserId,
                PermissionId = up.PermissionId
                // Có thể bổ sung UserName, PermissionName nếu cần join
            });
        }

        public async Task AddAsync(UserPermissionCreateRequest request)
        {
            var entity = new UserPermission
            {
                UserId = request.UserId,
                PermissionId = request.PermissionId
            };
            await _userPermissionRepository.AddAsync(entity);
        }

        public async Task DeleteAsync(int userPermissionId)
        {
            await _userPermissionRepository.DeleteAsync(userPermissionId);
        }

        public async Task<IEnumerable<UserPermissionResponse>> SearchAsync(int? userId, int? permissionId)
        {
            var list = await _userPermissionRepository.SearchAsync(userId, permissionId);
            return list.Select(up => new UserPermissionResponse
            {
                UserPermissionId = up.UserPermissionId,
                UserId = up.UserId,
                PermissionId = up.PermissionId
            });
        }

        public async Task UpdateAsync(UserPermissionUpdateRequest request)
        {
            // Giả sử repository có phương thức UpdateAsync(UserPermission entity)
            var entity = await _userPermissionRepository.GetByIdAsync(request.UserPermissionId);
            if (entity == null) throw new KeyNotFoundException("UserPermission không tồn tại");
            if (request.PermissionId.HasValue) entity.PermissionId = request.PermissionId.Value;
            if (request.UserId.HasValue) entity.UserId = request.UserId.Value;
            await _userPermissionRepository.UpdateAsync(entity);
        }
    }
}