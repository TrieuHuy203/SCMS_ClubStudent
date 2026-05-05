using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.Interfaces.iService;
using SCMS.DomainEntities.Enums; 
using SCMS.WebAPI.Attributes; // using directive for the custom PermissionAttribute
namespace SCMS.WebAPI.Controllers.Admin
{
    /// <summary>
    /// Controller quản lý quyền (Permission) cho admin.
    /// </summary>
    [ApiController]
    [Route("api/admin/permissions")]
    public class AdminPermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        /// <summary>
        /// Inject service vào controller.
        /// </summary>
        public AdminPermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        /// <summary>
        /// Lấy danh sách tất cả quyền.
        /// </summary>
        /// <returns>Danh sách quyền</returns>
         [Permission(AppPermission.Admin_Permission_View_List)]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _permissionService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        /// Lấy quyền theo Id.
        /// </summary>
        [Permission(AppPermission.Admin_Permission_View_Detail)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _permissionService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        /// <summary>
        /// Tạo mới quyền.
        /// </summary>
        [Permission(AppPermission.Admin_Permission_Create)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PermissionCreateRequest request)
        {
            var result = await _permissionService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.PermissionId }, result);
        }

        /// <summary>
        /// Cập nhật quyền.
        /// </summary>
        [Permission(AppPermission.Admin_Permission_Update)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PermissionUpdateRequest request)
        {
            var success = await _permissionService.UpdateAsync(id, request);
            if (!success) return NotFound();
            return NoContent();
        }

        /// <summary>
        /// Xóa quyền.
        /// </summary>
        [Permission(AppPermission.Admin_Permission_Delete)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _permissionService.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        /// <summary>
        /// Tìm kiếm quyền theo tên.
        /// </summary>
        [Permission(AppPermission.Admin_Permission_Search)]
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string name)
        {
            var result = await _permissionService.SearchByNameAsync(name);
            return Ok(result);
        }
    }
}