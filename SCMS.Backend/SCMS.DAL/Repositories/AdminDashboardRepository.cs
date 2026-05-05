using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Responses;
using SCMS.Contracts.Interfaces.iRepositores;
using Microsoft.EntityFrameworkCore;
using SCMS.DomainEntities.Entities;
using SCMS.DomainEntities.Enums;


namespace SCMS.DAL.Repositories
{
public class AdminDashboardRepository : IAdminDashboardRepository
{
    private readonly SCMSDbContext _context;

    public AdminDashboardRepository(SCMSDbContext context)
    {
        _context = context;
    }

    public async Task<AdminDashboardOverviewDto> GetOverviewAsync()
    {
        // Truy vấn tổng quan hệ thống
        return new AdminDashboardOverviewDto
        {
            TotalUsers = await _context.Users.CountAsync(),
            TotalClubs = await _context.Clubs.CountAsync(),
            TotalEvents = await _context.Events.CountAsync(),
            TotalPosts = await _context.Posts.CountAsync(),
            TotalComments = await _context.Comments.CountAsync(),
            TotalFeedbacks = await _context.Feedbacks.CountAsync(),
            TotalReports = await _context.PostReports.CountAsync()
        };
    }

    public async Task<AdminDashboardTodayMetricsDto> GetTodayMetricsAsync(DateTime today)
    {
        // Truy vấn số liệu hôm nay
        var start = today.Date;
        var end = start.AddDays(1);

        return new AdminDashboardTodayMetricsDto
        {
            NewUsersToday = await _context.Users.CountAsync(u => u.CreatedAt >= start && u.CreatedAt < end),
            NewPostsToday = await _context.Posts.CountAsync(p => p.CreatedAt >= start && p.CreatedAt < end),
            NewEventsToday = await _context.Events.CountAsync(e => e.CreatedAt >= start && e.CreatedAt < end),
            NewReportsToday = await _context.PostReports.CountAsync(r => r.CreatedAt >= start && r.CreatedAt < end),
            // ActiveUsersToday = await _context.UserActivities.CountAsync(a => a.Date == start)
        };
    }

    public async Task<AdminDashboardStatusBreakdownDto> GetStatusBreakdownAsync()
    {
        // Truy vấn trạng thái club, event, user
        return new AdminDashboardStatusBreakdownDto
        {
            ClubStatus = new ClubStatusBreakdown
            {
                Active = await _context.Clubs.CountAsync(c => c.Status == ClubStatus.Active.ToString()),
Disabled = await _context.Clubs.CountAsync(c => c.Status == ClubStatus.Disabled.ToString()),
Pending = await _context.Clubs.CountAsync(c => c.Status == ClubStatus.Pending.ToString())
            },
            EventStatus = new EventStatusBreakdown
            {
                Upcoming = await _context.Events.CountAsync(e => e.Status == EventStatus.Upcoming.ToString()),
                Ongoing = await _context.Events.CountAsync(e => e.Status == EventStatus.Ongoing.ToString()),
                Finished = await _context.Events.CountAsync(e => e.Status == EventStatus.Finished.ToString()),
                Cancelled = await _context.Events.CountAsync(e => e.Status == EventStatus.Cancelled.ToString())
            },
            UserStatus = new UserStatusBreakdown
            {
                Active = await _context.Users.CountAsync(u => u.Status == UserStatus.Active.ToString()),
                Locked = await _context.Users.CountAsync(u => u.Status == UserStatus.Locked.ToString())
            }
        };
    }

    public async Task<AdminDashboardGrowthChartDto> GetGrowthChartAsync(DateTime from, DateTime to)
    {
        // Group by ngày

        var userGrowth = await _context.Users
            .Where(u => u.CreatedAt != null && u.CreatedAt >= from && u.CreatedAt < to)
            .GroupBy(u => u.CreatedAt.Value.Date)
            .Select(g => new TimeSeriesData { Date = g.Key, Count = g.Count() })
            .OrderBy(x => x.Date)
            .ToListAsync();

        var clubGrowth = await _context.Clubs
            .Where(c => c.CreatedAt != null && c.CreatedAt >= from && c.CreatedAt < to)
            .GroupBy(c => c.CreatedAt.Value.Date)
            .Select(g => new TimeSeriesData { Date = g.Key, Count = g.Count() })
            .OrderBy(x => x.Date)
            .ToListAsync();

        var eventGrowth = await _context.Events
            .Where(e => e.CreatedAt != null && e.CreatedAt >= from && e.CreatedAt < to)
            .GroupBy(e => e.CreatedAt.Value.Date)
            .Select(g => new TimeSeriesData { Date = g.Key, Count = g.Count() })
            .OrderBy(x => x.Date)
            .ToListAsync();

        var postGrowth = await _context.Posts
            .Where(p => p.CreatedAt != null && p.CreatedAt >= from && p.CreatedAt < to)
            .GroupBy(p => p.CreatedAt.Value.Date)
            .Select(g => new TimeSeriesData { Date = g.Key, Count = g.Count() })
            .OrderBy(x => x.Date)
            .ToListAsync();

        var eventRegistrationGrowth = await _context.EventRegistrations
            .Where(er => er.CheckInTime != null && er.CheckInTime >= from && er.CheckInTime < to)
            .GroupBy(er => er.CheckInTime.Value.Date)
            .Select(g => new TimeSeriesData { Date = g.Key, Count = g.Count() })
            .OrderBy(x => x.Date)
            .ToListAsync();

        return new AdminDashboardGrowthChartDto
        {
            UserGrowth = userGrowth,
            ClubGrowth = clubGrowth,
            EventGrowth = eventGrowth,
            PostGrowth = postGrowth,
            EventRegistrationGrowth = eventRegistrationGrowth
        };
    }

    public async Task<AdminDashboardModerationDto> GetModerationAsync()
    {
        // Truy vấn các mục cần xử lý
        return new AdminDashboardModerationDto
        {
            PendingClubs = await _context.Clubs.CountAsync(c => c.Status == ClubStatus.Pending.ToString()),
            PendingEvents = await _context.Events.CountAsync(e => e.Status == EventStatus.Pending.ToString()),
            PendingPosts = await _context.Posts.CountAsync(p => p.Status == PostStatus.Pending.ToString()),
         UnresolvedReports = await _context.PostReports.CountAsync(r => r.Status == "Pending")
        };
    }

    public async Task<AdminDashboardTopRankingDto> GetTopRankingAsync()
    {
        // Top 5 CLB nhiều thành viên nhất
        var topClubsByMembers = await _context.Clubs
            .Select(c => new ClubRankingItem
            {
                ClubId = c.ClubId,
                ClubName = c.ClubName,
                Count = _context.Memberships.Count(m => m.ClubId == c.ClubId)
            })
            .OrderByDescending(x => x.Count)
            .Take(5)
            .ToListAsync();

        // Top 5 CLB nhiều sự kiện nhất
        var topClubsByEvents = await _context.Clubs
            .Select(c => new ClubRankingItem
            {
                ClubId = c.ClubId,
                ClubName = c.ClubName,
                Count = _context.Events.Count(e => e.ClubId == c.ClubId)
            })
            .OrderByDescending(x => x.Count)
            .Take(5)
            .ToListAsync();

        // Top 5 sự kiện nhiều người tham gia nhất
        var topEventsByParticipants = await _context.Events
            .Select(e => new EventRankingItem
            {
                EventId = e.EventId,
                EventName = e.EventName,
                ParticipantCount = _context.EventRegistrations.Count(er => er.EventId == e.EventId)
            })
            .OrderByDescending(x => x.ParticipantCount)
            .Take(5)
            .ToListAsync();

        // Top 5 bài post nhiều like + comment nhất
        var topPostsByEngagement = await _context.Posts
            .Select(p => new PostRankingItem
            {
                PostId = p.PostId,
                PostTitle = p.Title,
                EngagementCount = _context.Likes.Count(l => l.PostId == p.PostId) + _context.Comments.Count(c => c.PostId == p.PostId)
            })
            .OrderByDescending(x => x.EngagementCount)
            .Take(5)
            .ToListAsync();

        // Top 5 user hoạt động nhiều nhất (số post + comment)
        var topActiveUsers = await _context.Users
            .Select(u => new UserRankingItem
            {
                UserId = u.UserId,
                UserName = u.Username,
                ActivityScore = _context.Posts.Count(p => p.UserId == u.UserId) + _context.Comments.Count(c => c.UserId == u.UserId)
            })
            .OrderByDescending(x => x.ActivityScore)
            .Take(5)
            .ToListAsync();

        return new AdminDashboardTopRankingDto
        {
            TopClubsByMembers = topClubsByMembers,
            TopClubsByEvents = topClubsByEvents,
            TopEventsByParticipants = topEventsByParticipants,
            TopPostsByEngagement = topPostsByEngagement,
            TopActiveUsers = topActiveUsers
        };
    }

    public async Task<AdminDashboardRecentActivityDto> GetRecentActivityAsync()
    {
        // Truy vấn hoạt động gần đây (5 bản ghi mới nhất cho mỗi loại)
        var recentClubs = await _context.Clubs
            .OrderByDescending(c => c.CreatedAt)
            .Take(5)
            .Select(c => new ClubRecentItem
            {
                ClubId = c.ClubId,
                ClubName = c.ClubName,
                CreatedAt = c.CreatedAt ?? DateTime.MinValue
            })
            .ToListAsync();

        var recentEvents = await _context.Events
            .OrderByDescending(e => e.CreatedAt)
            .Take(5)
            .Select(e => new EventRecentItem
            {
                EventId = e.EventId,
                EventName = e.EventName,
                CreatedAt = e.CreatedAt ?? DateTime.MinValue
            })
            .ToListAsync();

        var recentPosts = await _context.Posts
            .OrderByDescending(p => p.CreatedAt)
            .Take(5)
            .Select(p => new PostRecentItem
            {
                PostId = p.PostId,
                PostTitle = p.Title,
                CreatedAt = p.CreatedAt ?? DateTime.MinValue
            })
            .ToListAsync();

        var recentReports = await _context.PostReports
            .OrderByDescending(r => r.CreatedAt)
            .Take(5)
            .Select(r => new ReportRecentItem
            {
                ReportId = r.PostReportId,
                ReportReason = r.Reason,
                CreatedAt = r.CreatedAt ?? DateTime.MinValue
            })
            .ToListAsync();

        return new AdminDashboardRecentActivityDto
        {
            RecentClubs = recentClubs,
            RecentEvents = recentEvents,
            RecentPosts = recentPosts,
            RecentReports = recentReports
        };
    }
}}