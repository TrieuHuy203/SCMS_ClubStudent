// Tổng quan hệ thống: các số liệu tổng hợp chính
public class AdminDashboardOverviewDto
{
    public int TotalUsers { get; set; } // Tổng số người dùng
    public int TotalClubs { get; set; } // Tổng số câu lạc bộ
    public int TotalEvents { get; set; } // Tổng số sự kiện
    public int TotalPosts { get; set; } // Tổng số bài đăng
    public int TotalComments { get; set; } // Tổng số bình luận
    public int TotalFeedbacks { get; set; } // Tổng số feedback
    public int TotalReports { get; set; } // Tổng số báo cáo vi phạm
}