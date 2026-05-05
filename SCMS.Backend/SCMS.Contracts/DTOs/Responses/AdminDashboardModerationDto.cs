// Thống kê các mục cần xử lý/kiểm duyệt
public class AdminDashboardModerationDto
{
    public int PendingClubs { get; set; } // Số CLB đang chờ duyệt
    public int PendingEvents { get; set; } // Số sự kiện chờ duyệt
    public int PendingPosts { get; set; } // Số bài post chờ duyệt
    public int UnresolvedReports { get; set; } // Số report chưa xử lý
}