// SCMS.BusinessService/Services/EventService.cs

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests.Event;
using SCMS.Contracts.DTOs.Responses;
using SCMS.Contracts.DTOs.Responses.Event;
using SCMS.Contracts.Interfaces.iRepositores;
using SCMS.Contracts.Interfaces.iService;
using SCMS.DomainEntities.Entities;

namespace SCMS.BusinessService.Services
{
    /// <summary>
    /// Cài đặt logic nghiệp vụ cho Event.
    /// </summary>
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IMembershipRepository _membershipRepository;
        private readonly IAuditLogService _auditLogService;

        public EventService(IEventRepository eventRepository, IMembershipRepository membershipRepository, IAuditLogService auditLogService)
        {
            _eventRepository = eventRepository;
            _membershipRepository = membershipRepository;
            _auditLogService = auditLogService;
        }

        public async Task<int> CreateEventAsync(CreateEventRequest request, int actorUserId)
        {
            if (string.IsNullOrWhiteSpace(request.EventName))
                throw new ArgumentException("Tên sự kiện không được để trống.");

            if (request.MaxParticipants.HasValue && request.MaxParticipants.Value <= 0)
                throw new ArgumentException("Số lượng tối đa người tham gia phải lớn hơn 0.");

            if (request.EventTime < DateTime.Now)
                throw new ArgumentException("Thời gian sự kiện phải lớn hơn hiện tại.");

            if (request.EndDateTime < request.EventTime)
                throw new ArgumentException("Ngày giờ kết thúc phải lớn hơn hoặc bằng ngày giờ bắt đầu sự kiện.");

            var createCandidates = await _eventRepository.GetListAsync(
                keyword: request.EventName,
                clubId: request.ClubId,
                page: 1,
                pageSize: 100);

            var existed = createCandidates.Items.Any(e =>
                    e.EventName == request.EventName &&
                    e.EventTime == request.EventTime &&
                    e.ClubId == request.ClubId);

            if (existed)
                throw new InvalidOperationException("Đã tồn tại sự kiện trùng tên, thời gian và CLB.");

            var entity = new Event
            {
                EventName = request.EventName,
                Description = request.Description,
                EventTime = request.EventTime,
                EndDateTime = request.EndDateTime,
                Location = request.Location,
                ClubId = request.ClubId,
                MaxParticipants = request.MaxParticipants,
                Status = "Pending" // Khi tạo mới, trạng thái là chờ duyệt
            };

            var result = await _eventRepository.AddAsync(entity);

            await SafeWriteAuditLogAsync(actorUserId, "Event", result.EventId, "CREATE");
            return result.EventId;
        }

        public async Task UpdateEventAsync(UpdateEventRequest request, int actorUserId)
        {
            if (string.IsNullOrWhiteSpace(request.EventName))
                throw new ArgumentException("Tên sự kiện không được để trống.");

            if (request.MaxParticipants.HasValue && request.MaxParticipants.Value <= 0)
                throw new ArgumentException("Số lượng tối đa người tham gia phải lớn hơn 0.");

            // Chỉ kiểm tra khi cập nhật từ phía người dùng (actorUserId > 0)
            if (actorUserId > 0 && request.EventTime < DateTime.Now)
                throw new ArgumentException("Thời gian sự kiện phải lớn hơn hiện tại.");

            if (request.EndDateTime < request.EventTime)
                throw new ArgumentException("Ngày giờ kết thúc phải lớn hơn hoặc bằng ngày giờ bắt đầu sự kiện.");

            var updateCandidates = await _eventRepository.GetListAsync(
                keyword: request.EventName,
                clubId: request.ClubId,
                page: 1,
                pageSize: 100);

            var existed = updateCandidates.Items.Any(e =>
                    e.EventId != request.EventId &&
                    e.EventName == request.EventName &&
                    e.EventTime == request.EventTime &&
                    e.ClubId == request.ClubId);

            if (existed)
                throw new InvalidOperationException("Đã tồn tại sự kiện trùng tên, thời gian và CLB.");

            var entity = await _eventRepository.GetByIdAsync(request.EventId);
            if (entity == null)
                throw new KeyNotFoundException("Không tìm thấy sự kiện.");

            var currentParticipantCount = entity.ParticipantCount ?? 0;
            if (request.MaxParticipants.HasValue && request.MaxParticipants.Value < currentParticipantCount)
                throw new InvalidOperationException("Số lượng tối đa không thể nhỏ hơn số người đã đăng ký hiện tại.");

            if (actorUserId == 0)
            {
                // Nếu là job hệ thống, chỉ cập nhật trạng thái
                entity.Status = request.Status;
            }
            else
            {
                entity.EventName = request.EventName;
                entity.Description = request.Description;
                entity.EventTime = request.EventTime;
                entity.EndDateTime = request.EndDateTime;
                entity.Location = request.Location;
                entity.ClubId = request.ClubId;
                entity.MaxParticipants = request.MaxParticipants;
                entity.Status = request.Status;
            }

            await _eventRepository.UpdateAsync(entity);

            await SafeWriteAuditLogAsync(actorUserId, "Event", entity.EventId, "UPDATE");
        }

        public async Task DeleteEventAsync(int eventId, int actorUserId)
        {
            var entity = await _eventRepository.GetByIdAsync(eventId);
            if (entity == null)
                throw new KeyNotFoundException("Không tìm thấy sự kiện.");

            await _eventRepository.DeleteAsync(eventId);

            await SafeWriteAuditLogAsync(actorUserId, "Event", eventId, "DELETE");
        }

        public async Task ApproveEventAsync(int eventId, int adminId)
        {
            var entity = await _eventRepository.GetByIdAsync(eventId);
            if (entity == null)
                throw new KeyNotFoundException("Không tìm thấy sự kiện.");

            if (entity.IsDeleted)
                throw new InvalidOperationException("Không thể duyệt sự kiện đã bị xóa.");

            if (string.Equals(entity.Status, "Cancelled", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Không thể duyệt sự kiện đã bị hủy.");

            entity.Status = "Approved";
            entity.RejectReason = null;
            entity.ApprovedAt = DateTime.Now;
            entity.ApprovedBy = adminId;

            await _eventRepository.UpdateAsync(entity);

            // Ghi nhận admin đã duyệt sự kiện.
            await SafeWriteAuditLogAsync(adminId, "Event", eventId, "APPROVE");
        }

        public async Task RejectEventAsync(int eventId, int adminId, string rejectReason)
        {
            if (string.IsNullOrWhiteSpace(rejectReason))
                throw new ArgumentException("Lý do từ chối không được để trống.");

            var entity = await _eventRepository.GetByIdAsync(eventId);
            if (entity == null)
                throw new KeyNotFoundException("Không tìm thấy sự kiện.");

            if (entity.IsDeleted)
                throw new InvalidOperationException("Không thể từ chối sự kiện đã bị xóa.");

            if (string.Equals(entity.Status, "Cancelled", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Không thể từ chối sự kiện đã bị hủy.");

            entity.Status = "Rejected";
            entity.RejectReason = rejectReason.Trim();
            entity.ApprovedAt = null;
            entity.ApprovedBy = null;

            await _eventRepository.UpdateAsync(entity);

            // Lưu cả lý do từ chối vào NewValue để tiện tra cứu audit.
            await SafeWriteAuditLogAsync(adminId, "Event", eventId, "REJECT", null, rejectReason.Trim());
        }

        public async Task<EventDetailResponse?> GetEventDetailAsync(int eventId)
        {
            var entity = await _eventRepository.GetByIdAsync(eventId);
            if (entity == null) return null;

            if (ShouldAutoComplete(entity))
            {
                entity.Status = "Completed";
                await _eventRepository.UpdateAsync(entity);
            }

            return new EventDetailResponse
            {
                EventId = entity.EventId,
                EventName = entity.EventName ?? string.Empty,
                Description = entity.Description ?? string.Empty,
                EventTime = entity.EventTime,
                EndDateTime = entity.EndDateTime,
                Location = entity.Location ?? string.Empty,
                Status = entity.Status ?? string.Empty,
                ClubName = entity.Club?.ClubName ?? string.Empty,
                CreatedAt = entity.CreatedAt ?? DateTime.MinValue,
                ParticipantCount = entity.ParticipantCount ?? 0,
                MaxParticipants = entity.MaxParticipants
            };
        }

        public async Task<PagedResult<EventListItemResponse>> GetEventListAsync(string? keyword = null, int? clubId = null, int page = 1, int pageSize = 10)
        {
            var (items, totalCount) = await _eventRepository.GetListAsync(keyword, clubId, page, pageSize);

            foreach (var item in items)
            {
                if (ShouldAutoComplete(item))
                {
                    item.Status = "Completed";
                    await _eventRepository.UpdateAsync(item);
                }
            }

            var list = items.Select(entity => new EventListItemResponse
            {
                EventId = entity.EventId,
                EventName = entity.EventName ?? string.Empty,
                EventTime = entity.EventTime,
                EndDateTime = entity.EndDateTime,
                Location = entity.Location ?? string.Empty,
                Status = entity.Status ?? string.Empty,
                ClubName = entity.Club?.ClubName ?? string.Empty,
                ParticipantCount = entity.ParticipantCount ?? 0,
                MaxParticipants = entity.MaxParticipants
            }).ToList();

            return new PagedResult<EventListItemResponse>
            {
                Items = list,
                TotalCount = totalCount,
                Page = page < 1 ? 1 : page,
                PageSize = pageSize < 1 ? 10 : pageSize
            };
        }

        public async Task<PagedResult<EventListItemResponse>> GetMyClubEventsAsync(int userId, string? keyword = null, int page = 1, int pageSize = 10)
        {
            var joinedClubIds = await _membershipRepository.GetApprovedClubIdsByUserAsync(userId);

            if (joinedClubIds.Count == 0)
            {
                return new PagedResult<EventListItemResponse>
                {
                    Items = new List<EventListItemResponse>(),
                    TotalCount = 0,
                    Page = page < 1 ? 1 : page,
                    PageSize = pageSize < 1 ? 10 : pageSize
                };
            }

            var (items, totalCount) = await _eventRepository.GetListByClubIdsAsync(joinedClubIds, keyword, page, pageSize);

            foreach (var item in items)
            {
                if (ShouldAutoComplete(item))
                {
                    item.Status = "Completed";
                    await _eventRepository.UpdateAsync(item);
                }
            }

            var list = items.Select(entity => new EventListItemResponse
            {
                EventId = entity.EventId,
                EventName = entity.EventName ?? string.Empty,
                EventTime = entity.EventTime,
                EndDateTime = entity.EndDateTime,
                Location = entity.Location ?? string.Empty,
                Status = entity.Status ?? string.Empty,
                ClubName = entity.Club?.ClubName ?? string.Empty,
                ParticipantCount = entity.ParticipantCount ?? 0,
                MaxParticipants = entity.MaxParticipants
            }).ToList();

            return new PagedResult<EventListItemResponse>
            {
                Items = list,
                TotalCount = totalCount,
                Page = page < 1 ? 1 : page,
                PageSize = pageSize < 1 ? 10 : pageSize
            };
        }

        public Task<PagedResult<EventListItemResponse>> SearchEventAsync(string keyword, int? clubId = null, int page = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                throw new ArgumentException("Từ khóa tìm kiếm không được để trống.");
            }

            return GetEventListAsync(keyword.Trim(), clubId, page, pageSize);
        }

        private static bool ShouldAutoComplete(Event entity)
        {
            if (!entity.EndDateTime.HasValue)
            {
                return false;
            }

            var status = entity.Status ?? string.Empty;
            var canAutoComplete =
                status.Equals("Upcoming", StringComparison.OrdinalIgnoreCase) ||
                status.Equals("Ongoing", StringComparison.OrdinalIgnoreCase) ||
                string.IsNullOrWhiteSpace(status);

            return canAutoComplete && entity.EndDateTime.Value <= DateTime.Now;
        }

        // Best effort: lỗi audit log không được làm hỏng nghiệp vụ chính.
        private async Task SafeWriteAuditLogAsync(int actorUserId, string tableName, int recordId, string actionType, string? oldValue = null, string? newValue = null)
        {
            try
            {
                await _auditLogService.LogAuditAsync(actorUserId, tableName, recordId, actionType, oldValue, newValue);
            }
            catch
            {
                // Intentionally swallow to preserve business flow.
            }
       
        }
    }
}