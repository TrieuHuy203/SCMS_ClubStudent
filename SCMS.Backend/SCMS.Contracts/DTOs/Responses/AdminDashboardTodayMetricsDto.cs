// Thống kê hoạt động trong ngày hôm nay
public class AdminDashboardTodayMetricsDto
{
    public int NewUsersToday { get; set; } // Số user đăng ký hôm nay
    public int NewPostsToday { get; set; } // Số bài post mới hôm nay
    public int NewEventsToday { get; set; } // Số sự kiện được tạo hôm nay
    public int NewReportsToday { get; set; } // Số báo cáo vi phạm hôm nay
    public int ActiveUsersToday { get; set; } // Số user active hôm nay
}