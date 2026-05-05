using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Responses;

public interface IAdminDashboardRepository
{
    // Tổng quan hệ thống
    Task<AdminDashboardOverviewDto> GetOverviewAsync();

    // Hoạt động hôm nay
    Task<AdminDashboardTodayMetricsDto> GetTodayMetricsAsync(DateTime today);

    // Trạng thái hệ thống
    Task<AdminDashboardStatusBreakdownDto> GetStatusBreakdownAsync();

    // Thống kê tăng trưởng
    Task<AdminDashboardGrowthChartDto> GetGrowthChartAsync(DateTime from, DateTime to);

    // Cần xử lý
    Task<AdminDashboardModerationDto> GetModerationAsync();

    // Top/xếp hạng
    Task<AdminDashboardTopRankingDto> GetTopRankingAsync();

    // Hoạt động gần đây
    Task<AdminDashboardRecentActivityDto> GetRecentActivityAsync();
}