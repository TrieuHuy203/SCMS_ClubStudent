using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.Interfaces.iService;
using SCMS.DomainEntities.Enums; // using directive for AppPermission enum
using SCMS.WebAPI.Attributes; // using directive for the custom PermissionAttribute
namespace SCMS.WebAPI.Controllers.Admin
{
    /// <summary>
    /// Controller quản lý UserPermission cho admin.
    /// </summary>
    [ApiController]
    [Route("api/admin/user-permissions")]
    public class AdminUserPermissionController : ControllerBase
    {
        private readonly IUserPermissionService _userPermissionService;

        public AdminUserPermissionController(IUserPermissionService userPermissionService)
        {
            _userPermissionService = userPermissionService;
        }

        /// <summary>
        /// Lấy tất cả UserPermission.
        /// </summary>
        /// <returns>Danh sách UserPermission</returns>
         [Permission(AppPermission.Admin_User_Permission_View_List)]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _userPermissionService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        /// Gán quyền cho user.
        /// </summary>
        /// <returns>Trả về 200 OK nếu gán quyền thành công</returns>
         [Permission(AppPermission.Admin_User_Permission_Add)]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] UserPermissionCreateRequest request)
        {
            await _userPermissionService.AddAsync(request);
            return Ok();
        }

        /// <summary>
        /// Xóa quyền khỏi user.
        /// </summary>
        [Permission(AppPermission.Admin_User_Permission_Delete)]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] int userPermissionId)
        {
            await _userPermissionService.DeleteAsync(userPermissionId);
            return NoContent();
        }

        /// <summary>
        /// Tìm kiếm UserPermission theo UserId hoặc PermissionId.
        /// </summary>
        [Permission(AppPermission.Admin_User_Permission_Search)]
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] int? userId, [FromQuery] int? permissionId)
        {
            var result = await _userPermissionService.SearchAsync(userId, permissionId);
            return Ok(result);
        }
        
        [Permission(AppPermission.Admin_User_Permission_Update)]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UserPermissionUpdateRequest request)
        {
            await _userPermissionService.UpdateAsync(request);
            return Ok();
        }
    }
}