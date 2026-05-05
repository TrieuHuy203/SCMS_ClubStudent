using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.Interfaces.iService;
using SCMS.WebAPI.Attributes; // using directive for the custom PermissionAttribute
using SCMS.DomainEntities.Enums;

namespace SCMS.WebAPI.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/auth-logs")]
    public class AdminAuthLogController : ControllerBase
    {
        private readonly IAuthLogService _authLogService;

        public AdminAuthLogController(IAuthLogService authLogService)
        {
            _authLogService = authLogService;
        }

        // GET: api/admin/auth-logs
         [Permission(AppPermission.Admin_AuthLog_View)]
        [HttpGet]
        public async Task<IActionResult> GetAuthLogs([FromQuery] AuthLogSearchRequest request)
        {
            var result = await _authLogService.GetAuthLogsAsync(request);
            return Ok(result);
        }
    }
}