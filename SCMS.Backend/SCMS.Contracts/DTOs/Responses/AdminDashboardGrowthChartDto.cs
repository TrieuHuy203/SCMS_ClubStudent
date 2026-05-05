// Thống kê tăng trưởng theo thời gian (dùng cho biểu đồ)
public class AdminDashboardGrowthChartDto
{
    public List<TimeSeriesData> UserGrowth { get; set; } // Số user đăng ký theo thời gian
    public List<TimeSeriesData> ClubGrowth { get; set; } // Số CLB được tạo theo thời gian
    public List<TimeSeriesData> EventGrowth { get; set; } // Số sự kiện theo thời gian
    public List<TimeSeriesData> PostGrowth { get; set; } // Số bài post theo thời gian
    public List<TimeSeriesData> EventRegistrationGrowth { get; set; } // Số đăng ký tham gia event theo thời gian
}

// Dữ liệu chuỗi thời gian
public class TimeSeriesData
{
    public DateTime Date { get; set; } // Ngày/tháng/năm
    public int Count { get; set; } // Số lượng
}