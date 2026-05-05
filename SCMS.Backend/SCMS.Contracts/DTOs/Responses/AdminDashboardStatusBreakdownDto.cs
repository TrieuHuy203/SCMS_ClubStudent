// Thống kê trạng thái các thực thể chính
public class AdminDashboardStatusBreakdownDto
{
    public ClubStatusBreakdown ClubStatus { get; set; }
    public EventStatusBreakdown EventStatus { get; set; }
    public UserStatusBreakdown UserStatus { get; set; }
}

// Trạng thái club
public class ClubStatusBreakdown
{
    public int Active { get; set; }
    public int Disabled { get; set; }
    public int Pending { get; set; }
}

// Trạng thái event
public class EventStatusBreakdown
{
    public int Upcoming { get; set; }
    public int Ongoing { get; set; }
    public int Finished { get; set; }
    public int Cancelled { get; set; }
}

// Trạng thái user
public class UserStatusBreakdown
{
    public int Active { get; set; }
    public int Locked { get; set; }
}