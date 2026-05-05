// Thống kê hoạt động gần đây
public class AdminDashboardRecentActivityDto
{
    public List<ClubRecentItem> RecentClubs { get; set; } // CLB mới tạo gần đây
    public List<EventRecentItem> RecentEvents { get; set; } // Event mới tạo gần đây
    public List<PostRecentItem> RecentPosts { get; set; } // Post mới đăng gần đây
    public List<ReportRecentItem> RecentReports { get; set; } // Report mới
}

// Các class phụ cho recent activity
public class ClubRecentItem
{
    public int ClubId { get; set; }
    public string ClubName { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class EventRecentItem
{
    public int EventId { get; set; }
    public string EventName { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class PostRecentItem
{
    public int PostId { get; set; }
    public string PostTitle { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ReportRecentItem
{
    public int ReportId { get; set; }
    public string ReportReason { get; set; }
    public DateTime CreatedAt { get; set; }
}