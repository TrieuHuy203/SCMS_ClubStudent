// SCMS.Contracts/DTOs/Responses/Event/EventListItemResponse.cs

using System;

namespace SCMS.Contracts.DTOs.Responses.Event
{
    /// <summary>
    /// DTO trả về thông tin tóm tắt của sự kiện trong danh sách (list).
    /// </summary>
    public class EventListItemResponse
    {
        /// <summary>
        /// Id của sự kiện.
        /// </summary>
        public int EventId { get; set; }

        /// <summary>
        /// Tên sự kiện.
        /// </summary>
        public required string EventName { get; set; }

        /// <summary>
        /// Thời gian diễn ra sự kiện.
        /// </summary>
        public DateTime EventTime { get; set; }

        /// <summary>
        /// Ngày giờ kết thúc sự kiện.
        /// </summary>
        public DateTime? EndDateTime { get; set; }

        /// <summary>
        /// Địa điểm tổ chức sự kiện.
        /// </summary>
        public required string Location { get; set; }

        /// <summary>
        /// Trạng thái sự kiện (Upcoming, Finished, ...).
        /// </summary>
        public required string Status { get; set; }

        /// <summary>
        /// Tên CLB tổ chức sự kiện (nếu cần hiển thị).
        /// </summary>
        public required string ClubName { get; set; }

        /// <summary>
        /// Số lượng người đang đăng ký tham gia sự kiện.
        /// </summary>
        public int ParticipantCount { get; set; }

        /// <summary>
        /// Số lượng tối đa người tham gia (null = không giới hạn).
        /// </summary>
        public int? MaxParticipants { get; set; }
    }
}