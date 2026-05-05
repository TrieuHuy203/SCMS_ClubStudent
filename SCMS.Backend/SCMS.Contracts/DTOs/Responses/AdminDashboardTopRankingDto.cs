// Thống kê top/xếp hạng
public class AdminDashboardTopRankingDto
{
    public List<ClubRankingItem> TopClubsByMembers { get; set; } // Top CLB nhiều thành viên nhất
    public List<ClubRankingItem> TopClubsByEvents { get; set; } // Top CLB nhiều sự kiện nhất
    public List<EventRankingItem> TopEventsByParticipants { get; set; } // Top sự kiện nhiều người tham gia nhất
    public List<PostRankingItem> TopPostsByEngagement { get; set; } // Top bài post nhiều like/comment nhất
    public List<UserRankingItem> TopActiveUsers { get; set; } // Top user hoạt động nhiều nhất
}

// Các class phụ cho ranking
public class ClubRankingItem
{
    public int ClubId { get; set; }
    public string ClubName { get; set; }
    public int Count { get; set; } // Số thành viên hoặc số sự kiện
}

public class EventRankingItem
{
    public int EventId { get; set; }
    public string EventName { get; set; }
    public int ParticipantCount { get; set; }
}

public class PostRankingItem
{
    public int PostId { get; set; }
    public string PostTitle { get; set; }
    public int EngagementCount { get; set; } // Tổng like/comment
}

public class UserRankingItem
{
    public int UserId { get; set; }
    public string UserName { get; set; }
    public int ActivityScore { get; set; }
}