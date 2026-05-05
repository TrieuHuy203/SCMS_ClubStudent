using Microsoft.AspNetCore.Mvc;
using SCMS.Contracts.DTOs.Responses;
using SCMS.Contracts.Interfaces.iService;
using SCMS.DomainEntities.Entities;
using SCMS.WebAPI.Attributes; // using directive for the custom PermissionAttribute
using SCMS.DomainEntities.Enums; // using directive for AppPermission enum
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
        [Permission(AppPermission.Admin_Dashboard_View_Overview)]
        [HttpGet("overview")]
        public async Task<ActionResult<AdminDashboardOverviewDto>> GetOverview()
            => Ok(await _dashboardService.GetOverviewAsync());
        // Admin lấy số liệu hôm nay
        [Permission(AppPermission.Admin_Dashboard_View_Today_Metrics)]
        [HttpGet("today")]
        public async Task<ActionResult<AdminDashboardTodayMetricsDto>> GetTodayMetrics()
            => Ok(await _dashboardService.GetTodayMetricsAsync(DateTime.Today));
        // Admin lấy phân breakdown trạng thái
        [Permission(AppPermission.Admin_Dashboard_View_Status_Breakdown)]
        [HttpGet("status")]
        public async Task<ActionResult<AdminDashboardStatusBreakdownDto>> GetStatusBreakdown()
            => Ok(await _dashboardService.GetStatusBreakdownAsync());
        // Admin lấy biểu đồ tăng trưởng
        [Permission(AppPermission.Admin_Dashboard_View_Growth_Chart)]
        [HttpGet("growth")]
        public async Task<ActionResult<AdminDashboardGrowthChartDto>> GetGrowthChart([FromQuery] DateTime from, [FromQuery] DateTime to)
            => Ok(await _dashboardService.GetGrowthChartAsync(from, to));
        // Admin lấy mục cần xử lý
        [Permission(AppPermission.Admin_Dashboard_View_Moderation)]
        [HttpGet("moderation")]
        public async Task<ActionResult<AdminDashboardModerationDto>> GetModeration()
            => Ok(await _dashboardService.GetModerationAsync());
        // Admin lấy top/xếp hạng
        [Permission(AppPermission.Admin_Dashboard_View_Top_Ranking)]
        [HttpGet("top")]
        public async Task<ActionResult<AdminDashboardTopRankingDto>> GetTopRanking()
            => Ok(await _dashboardService.GetTopRankingAsync());
    //  Admin lấy hoạt động gần đây
        [Permission(AppPermission.Admin_Dashboard_View_Recent_Activity)]
        [HttpGet("recent")]
        public async Task<ActionResult<AdminDashboardRecentActivityDto>> GetRecentActivity()
            => Ok(await _dashboardService.GetRecentActivityAsync());
    }
}