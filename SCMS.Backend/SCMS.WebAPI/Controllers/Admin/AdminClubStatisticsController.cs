using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Responses;
using SCMS.Contracts.Interfaces.iService;

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

        // GET: api/admin/clubs/{clubId}/statistics
        [HttpGet("{clubId}/statistics")]
        public async Task<IActionResult> GetStatistics(int clubId)
        {
            var stats = await _clubService.GetStatisticsAsync(clubId);
            return Ok(stats);
        }
    }
}