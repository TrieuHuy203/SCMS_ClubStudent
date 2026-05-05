// SCMS.Contracts/DTOs/Requests/Event/UpdateEventRequest.cs

using System;

namespace SCMS.Contracts.DTOs.Requests.Event
{
    /// <summary>
    /// DTO dùng để nhận dữ liệu khi cập nhật thông tin sự kiện từ phía client gửi lên.
    /// </summary>
    public class UpdateEventRequest
    {
        /// <summary>
        /// Id của sự kiện cần cập nhật.
        /// </summary>
        public int EventId { get; set; }

        /// <summary>
        /// Tên sự kiện (có thể cập nhật).
        /// </summary>
        public required string EventName { get; set; }

        /// <summary>
        /// Mô tả sự kiện (có thể cập nhật).
        /// </summary>
        public required string Description { get; set; }

        /// <summary>
        /// Thời gian diễn ra sự kiện (có thể cập nhật).
        /// </summary>
        public DateTime EventTime { get; set; }

        /// <summary>
        /// Ngày giờ kết thúc sự kiện (có thể cập nhật).
        /// </summary>
        public DateTime EndDateTime { get; set; }

        /// <summary>
        /// Địa điểm tổ chức sự kiện (có thể cập nhật).
        /// </summary>
        public required string Location { get; set; }

        /// <summary>
        /// Id của CLB tổ chức sự kiện (có thể cập nhật nếu cần).
        /// </summary>
        public int ClubId { get; set; }

        /// <summary>
        /// Số lượng tối đa người tham gia (null = không giới hạn).
        /// </summary>
        public int? MaxParticipants { get; set; }

        /// <summary>
        /// Trạng thái sự kiện (có thể cập nhật nếu cần).
        /// </summary>
        public string? Status { get; set; }

        // Có thể bổ sung các trường khác nếu muốn cho phép cập nhật thêm (Tags, ...)
    }
}