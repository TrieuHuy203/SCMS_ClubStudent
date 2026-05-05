using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Responses;
using SCMS.Contracts.Interfaces.iService;
using SCMS.DomainEntities.Entities;
using SCMS.WebAPI.Attributes; // using directive for the custom PermissionAttribute
using SCMS.DomainEntities.Enums; // using directive for AppPermission enum
namespace SCMS.WebAPI.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/clubs")]
    public class AdminClubStatisticsController : ControllerBase
    {
        private readonly IClubService _clubService;

        public AdminClubStatisticsController(IClubService clubService)
        {
            _clubService = clubService;
        }
        [Permission(AppPermission.Admin_Club_Statistics_View)]
        // GET: api/admin/clubs/{clubId}/statistics
        [HttpGet("{clubId}/statistics")]
        public async Task<IActionResult> GetStatistics(int clubId)
        {
            var stats = await _clubService.GetStatisticsAsync(clubId);
            return Ok(stats);
        }
    }
}