// SCMS.Contracts/DTOs/Requests/Event/CreateEventRequest.cs

using System;

namespace SCMS.Contracts.DTOs.Requests.Event
{
    /// <summary>
    /// DTO dùng để nhận dữ liệu khi tạo mới sự kiện từ phía client gửi lên.
    /// </summary>
    public class CreateEventRequest
    {
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
        public DateTime EndDateTime { get; set; }

        /// <summary>
        /// Địa điểm tổ chức sự kiện.
        /// </summary>
        public required string Location { get; set; }

        /// <summary>
        /// Id của CLB tổ chức sự kiện.
        /// </summary>
        public int ClubId { get; set; }

        /// <summary>
        /// Số lượng tối đa người tham gia (null = không giới hạn).
        /// </summary>
        public int? MaxParticipants { get; set; }
        
        // Có thể bổ sung các trường khác nếu cần, ví dụ: Status, Tags, ...
    }
}