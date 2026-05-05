using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.Interfaces.iService;
using SCMS.DomainEntities.Enums; // using directive for AppPermission enum
using SCMS.WebAPI.Attributes; // using directive for the custom PermissionAttribute
namespace SCMS.WebAPI.Controllers.Admin
{
    /// <summary>
    /// Controller quản lý UserRole cho admin.
    /// </summary>
    [ApiController]
    [Route("api/admin/user-roles")]
    public class AdminUserRoleController : ControllerBase
    {
        private readonly IUserRoleService _userRoleService;

        public AdminUserRoleController(IUserRoleService userRoleService)
        {
            _userRoleService = userRoleService;
        }

        /// <summary>
        /// Lấy tất cả UserRole.
        /// </summary>
        /// /// <returns>Danh sách UserRole</returns>
         [Permission(AppPermission.Admin_User_Role_View_List)]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _userRoleService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        /// Gán vai trò cho user.
        /// </summary>
        /// <returns>Trả về 200 OK nếu gán vai trò thành công</returns>
         [Permission(AppPermission.Admin_User_Role_Add)]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] UserRoleCreateRequest request)
        {
            await _userRoleService.AddAsync(request);
            return Ok();
        }

        /// <summary>
        /// Xóa vai trò khỏi user.
        /// </summary>
        /// <returns>Trả về 204 NoContent nếu xóa vai trò thành công</returns>
         [Permission(AppPermission.Admin_User_Role_Delete)]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] int userRoleId)
        {
            await _userRoleService.DeleteAsync(userRoleId);
            return NoContent();
        }

        /// <summary>
        /// Tìm kiếm UserRole theo UserId hoặc RoleId.
        /// </summary>
        /// <returns>Danh sách UserRole thỏa mãn điều kiện tìm kiếm</returns>
         [Permission(AppPermission.Admin_User_Role_Search)]
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] int? userId, [FromQuery] int? roleId)
        {
            var result = await _userRoleService.SearchAsync(userId, roleId);
            return Ok(result);
        }
    }
}