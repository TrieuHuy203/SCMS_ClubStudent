using System;

namespace SCMS.Contracts.DTOs.Responses
{
    public class AdminEventRegistrationDetailResponse
    {
        public int EventRegistrationId { get; set; }

        public int EventId { get; set; }
        public string? EventName { get; set; }
        public string? EventDescription { get; set; }
        public DateTime EventTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public string? EventLocation { get; set; }
        public string? EventStatus { get; set; }

        public int ClubId { get; set; }
        public string? ClubName { get; set; }

        public int UserId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }

        public string? RegistrationStatus { get; set; }
        public DateTime? CheckInTime { get; set; }
    }
}