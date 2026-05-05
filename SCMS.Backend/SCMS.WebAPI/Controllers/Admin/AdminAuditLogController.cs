using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.Interfaces.iService;
using SCMS.WebAPI.Attributes; // using directive for the custom PermissionAttribute
using SCMS.DomainEntities.Enums;


namespace SCMS.WebAPI.Controllers.Admin
{
    [ApiController]
    
    [Route("api/admin/audit-logs")]
    public class AdminAuditLogController : ControllerBase
    {
        private readonly IAuditLogService _auditLogService; 

        public AdminAuditLogController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService; // Dependency injection of the audit log service
        }

        // GET: api/admin/audit-logs
      [Permission(AppPermission.Admin_AuditLog_View)]
        [HttpGet]
        public async Task<IActionResult> GetAuditLogs([FromQuery] AuditLogSearchRequest request)
        {
            var result = await _auditLogService.GetAuditLogsAsync(request);
            return Ok(result);
        }
    }
}