using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.Interfaces.iService;
using SCMS.DomainEntities.Enums; 
using SCMS.WebAPI.Attributes; // using directive for the custom PermissionAttribute
namespace SCMS.WebAPI.Controllers.Admin
{
    /// <summary>
    /// Controller quản lý RolePermission cho admin.
    /// </summary>
    [ApiController]
    [Route("api/admin/role-permissions")]
    public class AdminRolePermissionController : ControllerBase
    {
        private readonly IRolePermissionService _rolePermissionService;

        public AdminRolePermissionController(IRolePermissionService rolePermissionService)
        {
            _rolePermissionService = rolePermissionService;
        }

        /// <summary>
        /// Lấy tất cả RolePermission.
        /// </summary>
        /// <returns>Danh sách RolePermission</returns>
         [Permission(AppPermission.Admin_Role_Permission_View_List)]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _rolePermissionService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        /// Gán quyền cho vai trò.
        /// </summary>
        [Permission(AppPermission.Admin_Role_Permission_Add)]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] RolePermissionCreateRequest request)
        {
            await _rolePermissionService.AddAsync(request);
            return Ok();
        }

        /// <summary>
        /// Xóa quyền khỏi vai trò.
        /// </summary>
        [Permission(AppPermission.Admin_Role_Permission_Delete)]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] int roleId, [FromQuery] int permissionId)
        {
            await _rolePermissionService.DeleteAsync(roleId, permissionId);
            return NoContent();
        }

        /// <summary>
        /// Tìm kiếm RolePermission theo RoleId hoặc PermissionId.
        /// </summary>
        /// <returns>Danh sách RolePermission thỏa mãn điều kiện tìm kiếm</returns>
         [Permission(AppPermission.Admin_Role_Permission_Search)]
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] int? roleId, [FromQuery] int? permissionId)
        {
            var result = await _rolePermissionService.SearchAsync(roleId, permissionId);
            return Ok(result);
        }
    }
}