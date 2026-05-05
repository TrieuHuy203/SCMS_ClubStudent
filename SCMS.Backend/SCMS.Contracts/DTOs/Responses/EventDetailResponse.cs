// SCMS.Contracts/DTOs/Responses/Event/EventDetailResponse.cs

using System;

namespace SCMS.Contracts.DTOs.Responses.Event
{
    /// <summary>
    /// DTO trả về thông tin chi tiết của một sự kiện.
    /// </summary>
    public class EventDetailResponse
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
        /// Mô tả sự kiện.
        /// </summary>
        public required string Description { get; set; }

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
        /// Trạng thái sự kiện.
        /// </summary>
        public required string Status { get; set; }

        /// <summary>
        /// Tên CLB tổ chức sự kiện.
        /// </summary>
        public required string ClubName { get; set; }

        /// <summary>
        /// Ngày tạo sự kiện.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Số lượng người đang đăng ký tham gia sự kiện.
        /// </summary>
        public int ParticipantCount { get; set; }

        /// <summary>
        /// Số lượng tối đa người tham gia (null = không giới hạn).
        /// </summary>
        public int? MaxParticipants { get; set; }

        // Có thể bổ sung thêm các trường khác nếu cần, ví dụ: danh sách file đính kèm, tag, ...
    }
}