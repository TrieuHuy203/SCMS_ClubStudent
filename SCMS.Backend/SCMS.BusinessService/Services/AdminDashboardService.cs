using System;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Responses;
using SCMS.Contracts.Interfaces.iService;
using SCMS.Contracts.Interfaces.iRepositores;

public class AdminDashboardService : IAdminDashboardService
{
    private readonly IAdminDashboardRepository _dashboardRepo;

    public AdminDashboardService(IAdminDashboardRepository dashboardRepo)
    {
        _dashboardRepo = dashboardRepo;
    }


    public Task<AdminDashboardOverviewDto> GetOverviewAsync()
        => _dashboardRepo.GetOverviewAsync();

    public Task<AdminDashboardTodayMetricsDto> GetTodayMetricsAsync(DateTime today)
        => _dashboardRepo.GetTodayMetricsAsync(today);

    public Task<AdminDashboardStatusBreakdownDto> GetStatusBreakdownAsync()
        => _dashboardRepo.GetStatusBreakdownAsync();

    public Task<AdminDashboardGrowthChartDto> GetGrowthChartAsync(DateTime from, DateTime to)
        => _dashboardRepo.GetGrowthChartAsync(from, to);

    public Task<AdminDashboardModerationDto> GetModerationAsync()
        => _dashboardRepo.GetModerationAsync();

    public Task<AdminDashboardTopRankingDto> GetTopRankingAsync()
        => _dashboardRepo.GetTopRankingAsync();

    public Task<AdminDashboardRecentActivityDto> GetRecentActivityAsync()
        => _dashboardRepo.GetRecentActivityAsync();
}