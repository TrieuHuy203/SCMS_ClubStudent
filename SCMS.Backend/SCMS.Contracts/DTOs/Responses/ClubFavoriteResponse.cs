namespace SCMS.Contracts.DTOs.Responses
{
	/// <summary>
	/// DTO trả về một CLB đã được user yêu thích.
	/// </summary>
	public class ClubFavoriteResponse
	{
		public int ClubFavoriteId { get; set; } // Khóa chính của bản ghi yêu thích
		public int UserId { get; set; } // Khóa chính của user đã yêu thích
		public string? Username { get; set; } // Username của user
		public string? FullName { get; set; } // Họ tên user
		public string? Email { get; set; } // Email user
		public int ClubId { get; set; } // Khóa chính của CLB
		public string ClubName { get; set; } = null!; // Tên câu lạc bộ
		public string? Description { get; set; } // Mô tả câu lạc bộ
		public string? Field { get; set; } // Lĩnh vực hoạt động
		public string? School { get; set; } // Trường/đơn vị quản lý
		public int? MemberCount { get; set; } // Số thành viên hiện tại
		public DateTime? CreatedAt { get; set; } // Thời điểm user yêu thích CLB
	}
}
