// SCMS.Contracts/Interfaces/iService/IEventService.cs

using System.Collections.Generic;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests.Event;
using SCMS.Contracts.DTOs.Responses.Event;
using SCMS.Contracts.DTOs.Responses;

namespace SCMS.Contracts.Interfaces.iService
{
    /// <summary>
    /// Interface định nghĩa các phương thức nghiệp vụ cho Event.
    /// </summary>
    public interface IEventService
    {
        Task<int> CreateEventAsync(CreateEventRequest request, int actorUserId);
        Task UpdateEventAsync(UpdateEventRequest request, int actorUserId);
        Task DeleteEventAsync(int eventId, int actorUserId);
        Task ApproveEventAsync(int eventId, int adminId);
        Task RejectEventAsync(int eventId, int adminId, string rejectReason);
        Task<EventDetailResponse?> GetEventDetailAsync(int eventId);
        Task<PagedResult<EventListItemResponse>> GetEventListAsync(string? keyword = null, int? clubId = null, int page = 1, int pageSize = 10);
        Task<PagedResult<EventListItemResponse>> GetMyClubEventsAsync(int userId, string? keyword = null, int page = 1, int pageSize = 10);
        Task<PagedResult<EventListItemResponse>> SearchEventAsync(string keyword, int? clubId = null, int page = 1, int pageSize = 10);
    }
}