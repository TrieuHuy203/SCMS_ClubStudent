using System;
using System.Threading.Tasks;
using SCMS.Contracts.Interfaces.iService;

public class EventStatusJob
{
    private readonly IEventService _eventService;

    // Inject service qua constructor
    public EventStatusJob(IEventService eventService)
    {
        _eventService = eventService;
    }

    // Hàm này sẽ được Hangfire gọi định kỳ
 public async Task UpdateEventStatuses()
{
    // Lấy tất cả sự kiện (không phân trang, lấy hết)
        int page = 1;
        int pageSize = 100;
    bool hasMore = true;

    while (hasMore)
    {
        var result = await _eventService.GetEventListAsync(null, null, page, pageSize);
        var events = result.Items;
        hasMore = events.Count == pageSize;

        foreach (var ev in events)
        {
            // Nếu sự kiện đã bị huỷ, từ chối, hoặc đã hoàn thành thì bỏ qua
            if (ev.Status == "Cancelled" || ev.Status == "Rejected" || ev.Status == "Completed" || ev.Status == "Pending")
                continue;

            var now = DateTime.Now;
            var endDate = ev.EndDateTime ?? DateTime.MaxValue;

            // Lấy entity hiện tại từ DB để tránh lỗi ClubId hoặc các trường khác
            var entity = await _eventService.GetEventDetailAsync(ev.EventId);
            if (entity == null) continue;

            // Lấy các trường cần thiết, fallback sang ev nếu không có trong entity
            var eventName = entity.EventName ?? ev.EventName;
            var description = entity.Description ?? string.Empty;
            var eventTime = entity.EventTime != default(DateTime) ? entity.EventTime : ev.EventTime;
            var endDateTime = entity.EndDateTime ?? (ev.EndDateTime ?? DateTime.MaxValue);
            var location = entity.Location ?? ev.Location;
            var maxParticipants = entity.MaxParticipants ?? ev.MaxParticipants;
            // Không có ClubId trong entity và ev, nên bỏ khỏi request nếu không cần thiết

            // Nếu đã duyệt và đang trong thời gian diễn ra
            if (ev.Status == "Approved" && ev.EventTime <= now && endDate > now)
            {
                if (ev.Status != "Ongoing")
                {
                    var updateRequest = new SCMS.Contracts.DTOs.Requests.Event.UpdateEventRequest
                    {
                        EventId = ev.EventId,
                        EventName = eventName,
                        Description = description,
                        EventTime = eventTime,
                        EndDateTime = endDateTime,
                        Location = location,
                        MaxParticipants = maxParticipants,
                        Status = "Ongoing"
                    };
                    await _eventService.UpdateEventAsync(updateRequest, 0);
                }
            }
            // Nếu đã kết thúc
            else if ((ev.Status == "Approved" || ev.Status == "Ongoing") && endDate <= now)
            {
                if (ev.Status != "Completed")
                {
                    var updateRequest = new SCMS.Contracts.DTOs.Requests.Event.UpdateEventRequest
                    {
                        EventId = ev.EventId,
                        EventName = eventName,
                        Description = description,
                        EventTime = eventTime,
                        EndDateTime = endDateTime,
                        Location = location,
                        MaxParticipants = maxParticipants,
                        Status = "Completed"
                    };
                    await _eventService.UpdateEventAsync(updateRequest, 0);
                }
            }
        }

        page++;
    }
}
}