using System;

namespace SCMS.Contracts.DTOs.Responses
{
	// Item dùng cho danh sách sự kiện user đã đăng ký
	public class EventRegistrationItemResponse
	{
		public int EventRegistrationId { get; set; }
		public int EventId { get; set; }
		public int UserId { get; set; }

		public string? EventName { get; set; }
		public DateTime EventTime { get; set; }
		public DateTime? EndDateTime { get; set; }

		public int ClubId { get; set; }
		public string? ClubName { get; set; }

		public string? RegistrationStatus { get; set; }
		public DateTime? CheckInTime { get; set; }
	}
}
