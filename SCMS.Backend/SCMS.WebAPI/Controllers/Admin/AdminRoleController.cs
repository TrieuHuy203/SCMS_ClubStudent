using Microsoft.AspNetCore.Mvc;
using SCMS.Contracts.Interfaces.iService;
using System.Threading.Tasks;
using SCMS.DomainEntities.Enums; 
using SCMS.WebAPI.Attributes; // using directive for the custom PermissionAttribute
namespace SCMS.WebAPI.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/roles")]
    public class AdminRoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public AdminRoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [Permission(AppPermission.Admin_Role_View_List)]
        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return Ok(roles);
        }
        // Admin xem chi tiết một role
        // Admin tạo
        // Admin cập nhật
        // Admin xóa role
        
    }
}