using Microsoft.AspNetCore.Mvc;
using SCMS.Contracts.DTOs.Responses;
using SCMS.Contracts.Interfaces.iService;

namespace SCMS.WebAPI.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/dashboard")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IAdminDashboardService _dashboardService;

        public AdminDashboardController(IAdminDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

// Admin lấy tổng quan dashboard
        [HttpGet("overview")]
        public async Task<ActionResult<AdminDashboardOverviewDto>> GetOverview()
            => Ok(await _dashboardService.GetOverviewAsync());
// Admin lấy số liệu hôm nay
        [HttpGet("today")]
        public async Task<ActionResult<AdminDashboardTodayMetricsDto>> GetTodayMetrics()
            => Ok(await _dashboardService.GetTodayMetricsAsync(DateTime.Today));
// Admin lấy phân breakdown trạng thái
        [HttpGet("status")]
        public async Task<ActionResult<AdminDashboardStatusBreakdownDto>> GetStatusBreakdown()
            => Ok(await _dashboardService.GetStatusBreakdownAsync());
// Admin lấy biểu đồ tăng trưởng
        [HttpGet("growth")]
        public async Task<ActionResult<AdminDashboardGrowthChartDto>> GetGrowthChart([FromQuery] DateTime from, [FromQuery] DateTime to)
            => Ok(await _dashboardService.GetGrowthChartAsync(from, to));
// Admin lấy mục cần xử lý
        [HttpGet("moderation")]
        public async Task<ActionResult<AdminDashboardModerationDto>> GetModeration()
            => Ok(await _dashboardService.GetModerationAsync());
// Admin lấy top/xếp hạng
        [HttpGet("top")]
        public async Task<ActionResult<AdminDashboardTopRankingDto>> GetTopRanking()
            => Ok(await _dashboardService.GetTopRankingAsync());
//  Admin lấy hoạt động gần đây
        [HttpGet("recent")]
        public async Task<ActionResult<AdminDashboardRecentActivityDto>> GetRecentActivity()
            => Ok(await _dashboardService.GetRecentActivityAsync());
    }
}