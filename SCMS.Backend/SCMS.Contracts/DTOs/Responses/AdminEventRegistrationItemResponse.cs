using System;

namespace SCMS.Contracts.DTOs.Responses
{
    // Item dùng cho admin khi xem danh sách người đăng ký theo sự kiện
    public class AdminEventRegistrationItemResponse
    {
        public int EventRegistrationId { get; set; }
        public int EventId { get; set; }
        public string? EventName { get; set; }

        public int UserId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }

        public string? RegistrationStatus { get; set; }
        public DateTime? CheckInTime { get; set; }
    }
}