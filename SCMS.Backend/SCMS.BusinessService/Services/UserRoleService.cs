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
    /// Triển khai nghiệp vụ cho UserRole.
    /// </summary>
    public class UserRoleService : IUserRoleService
    {
        private readonly IUserRoleRepository _userRoleRepository;

        public UserRoleService(IUserRoleRepository userRoleRepository)
        {
            _userRoleRepository = userRoleRepository;
        }

        public async Task<IEnumerable<UserRoleResponse>> GetAllAsync()
        {
            var list = await _userRoleRepository.GetAllAsync();
            return list.Select(ur => new UserRoleResponse
            {
                UserRoleId = ur.UserRoleId,
                UserId = ur.UserId,
                RoleId = ur.RoleId
                // Có thể bổ sung UserName, RoleName nếu cần join
            });
        }

        public async Task AddAsync(UserRoleCreateRequest request)
        {
            var entity = new UserRole
            {
                UserId = request.UserId,
                RoleId = request.RoleId
            };
            await _userRoleRepository.AddAsync(entity);
        }

        public async Task DeleteAsync(int userRoleId)
        {
            await _userRoleRepository.DeleteAsync(userRoleId);
        }

        public async Task<IEnumerable<UserRoleResponse>> SearchAsync(int? userId, int? roleId)
        {
            var list = await _userRoleRepository.SearchAsync(userId, roleId);
            return list.Select(ur => new UserRoleResponse
            {
                UserRoleId = ur.UserRoleId,
                UserId = ur.UserId,
                RoleId = ur.RoleId
            });
        }
    }
}