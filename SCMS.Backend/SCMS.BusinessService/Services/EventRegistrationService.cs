using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.DTOs.Responses;
using SCMS.Contracts.Interfaces.iRepositores;
using SCMS.Contracts.Interfaces.iService;
using SCMS.DomainEntities.Entities;

namespace SCMS.BusinessService.Services
{
	public class EventRegistrationService : IEventRegistrationService
	{
		private readonly IEventRegistrationRepository _eventRegistrationRepository;

		public EventRegistrationService(IEventRegistrationRepository eventRegistrationRepository)
		{
			_eventRegistrationRepository = eventRegistrationRepository;
		}

		public async Task<EventRegistrationDetailResponse> RegisterAsync(EventRegisterRequest request, int userId)
		{
			if (request == null)
				throw new ArgumentNullException(nameof(request));

			var ev = await _eventRegistrationRepository.GetEventByIdAsync(request.EventId);
			if (ev == null)
				throw new KeyNotFoundException("Không tìm thấy sự kiện.");

			if (string.Equals(ev.Status, "Completed", StringComparison.OrdinalIgnoreCase) ||
				string.Equals(ev.Status, "Cancelled", StringComparison.OrdinalIgnoreCase))
			{
				throw new InvalidOperationException("Sự kiện đã kết thúc hoặc đã hủy, không thể đăng ký.");
			}

			if (ev.EndDateTime.HasValue && ev.EndDateTime.Value <= DateTime.Now)
			{
				throw new InvalidOperationException("Sự kiện đã hết hạn đăng ký.");
			}

			var isApprovedMember = await _eventRegistrationRepository.IsUserApprovedMemberOfClubAsync(userId, ev.ClubId);
			if (!isApprovedMember)
			{
				throw new InvalidOperationException("Bạn phải là thành viên đã được duyệt của CLB để đăng ký sự kiện này.");
			}

			var existed = await _eventRegistrationRepository.GetByUserAndEventAsync(userId, request.EventId);
			if (existed != null)
			{
				if (string.Equals(existed.RegistrationStatus, "Registered", StringComparison.OrdinalIgnoreCase))
				{
					throw new InvalidOperationException("Bạn đã đăng ký sự kiện này trước đó.");
				}

				if (string.Equals(existed.RegistrationStatus, "Cancelled", StringComparison.OrdinalIgnoreCase))
				{
					await EnsureEventHasCapacityAsync(ev);

					existed.RegistrationStatus = "Registered";
					existed.CheckInTime = null;

					var reRegistered = await _eventRegistrationRepository.UpdateAsync(existed);
					await _eventRegistrationRepository.SyncEventParticipantCountAsync(request.EventId);
					return MapToDetailResponse(reRegistered);
				}

				throw new InvalidOperationException("Đăng ký hiện tại không ở trạng thái hợp lệ để thao tác.");
			}

			await EnsureEventHasCapacityAsync(ev);

			var entity = new EventRegistration
			{
				UserId = userId,
				EventId = request.EventId,
				RegistrationStatus = "Registered",
				CheckInTime = null
			};

			var created = await _eventRegistrationRepository.AddAsync(entity);
			await _eventRegistrationRepository.SyncEventParticipantCountAsync(request.EventId);
			return MapToDetailResponse(created);
		}

		public async Task<PagedResult<EventRegistrationItemResponse>> GetMyRegistrationsAsync(
			int userId,
			EventRegistrationSearchRequest request)
		{
			request ??= new EventRegistrationSearchRequest();

			var page = request.Page < 1 ? 1 : request.Page;
			var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

			var (items, totalCount) = await _eventRegistrationRepository.GetMyRegistrationsAsync(
				userId,
				request.Keyword,
				request.RegistrationStatus,
				page,
				pageSize);

			var responses = items.Select(MapToItemResponse).ToList();

			return new PagedResult<EventRegistrationItemResponse>
			{
				Items = responses,
				TotalCount = totalCount,
				Page = page,
				PageSize = pageSize
			};
		}

		public async Task<EventRegistrationDetailResponse?> GetMyRegistrationDetailAsync(int eventRegistrationId, int userId)
		{
			var registration = await _eventRegistrationRepository.GetByIdAsync(eventRegistrationId, userId);
			if (registration == null)
			{
				throw new KeyNotFoundException("Không tìm thấy đăng ký sự kiện hoặc bạn không có quyền xem.");
			}

			return MapToDetailResponse(registration);
		}

		public async Task<EventRegistrationDetailResponse> CancelMyRegistrationAsync(int eventRegistrationId, int userId)
		{
			var registration = await _eventRegistrationRepository.GetByIdAsync(eventRegistrationId, userId);
			if (registration == null)
			{
				throw new KeyNotFoundException("Không tìm thấy đăng ký sự kiện hoặc bạn không có quyền hủy.");
			}

			if (!string.Equals(registration.RegistrationStatus, "Registered", StringComparison.OrdinalIgnoreCase))
			{
				throw new InvalidOperationException("Chỉ có thể hủy khi trạng thái đăng ký đang là Registered.");
			}

			registration.RegistrationStatus = "Cancelled";
			var updated = await _eventRegistrationRepository.UpdateAsync(registration);
			await _eventRegistrationRepository.SyncEventParticipantCountAsync(registration.EventId);
			return MapToDetailResponse(updated);
		}

		public async Task<PagedResult<AdminEventRegistrationItemResponse>> GetRegistrationsByEventAsync(
			int eventId,
			EventRegistrationSearchRequest request)
		{
			var ev = await _eventRegistrationRepository.GetEventByIdAsync(eventId);
			if (ev == null)
			{
				throw new KeyNotFoundException("Không tìm thấy sự kiện.");
			}

			request ??= new EventRegistrationSearchRequest();
			var page = request.Page < 1 ? 1 : request.Page;
			var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

			var (items, totalCount) = await _eventRegistrationRepository.GetRegistrationsByEventAsync(
				eventId,
				request.Keyword,
				request.RegistrationStatus,
				page,
				pageSize);

			var responses = items.Select(MapToAdminItemResponse).ToList();

			return new PagedResult<AdminEventRegistrationItemResponse>
			{
				Items = responses,
				TotalCount = totalCount,
				Page = page,
				PageSize = pageSize
			};
		}

		public async Task<PagedResult<AdminEventRegistrationItemResponse>> GetClubEventRegistrationsForMemberAsync(
			int userId,
			int eventId,
			EventRegistrationSearchRequest request)
		{
			var ev = await _eventRegistrationRepository.GetEventByIdAsync(eventId);
			if (ev == null)
			{
				throw new KeyNotFoundException("Không tìm thấy sự kiện.");
			}

			// Chỉ cho phép người thuộc CLB tổ chức sự kiện xem danh sách đăng ký.
			var isApprovedMember = await _eventRegistrationRepository.IsUserApprovedMemberOfClubAsync(userId, ev.ClubId);
			if (!isApprovedMember)
			{
				throw new UnauthorizedAccessException("Bạn không có quyền xem danh sách đăng ký của sự kiện này.");
			}

			request ??= new EventRegistrationSearchRequest();
			var page = request.Page < 1 ? 1 : request.Page;
			var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

			var (items, totalCount) = await _eventRegistrationRepository.GetRegistrationsByEventAsync(
				eventId,
				request.Keyword,
				request.RegistrationStatus,
				page,
				pageSize);

			var responses = items.Select(MapToAdminItemResponse).ToList();

			return new PagedResult<AdminEventRegistrationItemResponse>
			{
				Items = responses,
				TotalCount = totalCount,
				Page = page,
				PageSize = pageSize
			};
		}

		public async Task<PagedResult<AdminEventRegistrationItemResponse>> GetAllRegistrationsAsync(
			EventRegistrationSearchRequest request)
		{
			request ??= new EventRegistrationSearchRequest();
			var page = request.Page < 1 ? 1 : request.Page;
			var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

			var (items, totalCount) = await _eventRegistrationRepository.GetAllRegistrationsAsync(
				request.Keyword,
				request.RegistrationStatus,
				page,
				pageSize);

			var responses = items.Select(MapToAdminItemResponse).ToList();

			return new PagedResult<AdminEventRegistrationItemResponse>
			{
				Items = responses,
				TotalCount = totalCount,
				Page = page,
				PageSize = pageSize
			};
		}

		public async Task<AdminEventRegistrationDetailResponse> GetRegistrationDetailForAdminAsync(int eventRegistrationId)
		{
			var registration = await _eventRegistrationRepository.GetByIdForAdminAsync(eventRegistrationId);
			if (registration == null)
			{
				throw new KeyNotFoundException("Không tìm thấy đăng ký sự kiện.");
			}

			return MapToAdminDetailResponse(registration);
		}

		public async Task<byte[]> ExportRegistrationsByEventForAdminAsync(int eventId)
		{
			var ev = await _eventRegistrationRepository.GetEventByIdAsync(eventId);
			if (ev == null)
			{
				throw new KeyNotFoundException("Không tìm thấy sự kiện.");
			}

			var registrations = await _eventRegistrationRepository.GetRegistrationsForExportByEventAsync(eventId);
			return BuildRegistrationsCsv(registrations);
		}

		public async Task<byte[]> ExportClubEventRegistrationsForMemberAsync(int userId, int eventId)
		{
			var ev = await _eventRegistrationRepository.GetEventByIdAsync(eventId);
			if (ev == null)
			{
				throw new KeyNotFoundException("Không tìm thấy sự kiện.");
			}

			var isApprovedMember = await _eventRegistrationRepository.IsUserApprovedMemberOfClubAsync(userId, ev.ClubId);
			if (!isApprovedMember)
			{
				throw new UnauthorizedAccessException("Bạn không có quyền export danh sách đăng ký của sự kiện này.");
			}

			var registrations = await _eventRegistrationRepository.GetRegistrationsForExportByEventAsync(eventId);
			return BuildRegistrationsCsv(registrations);
		}

		private static EventRegistrationItemResponse MapToItemResponse(EventRegistration entity)
		{
			return new EventRegistrationItemResponse
			{
				EventRegistrationId = entity.EventRegistrationId,
				EventId = entity.EventId,
				UserId = entity.UserId,
				EventName = entity.Event?.EventName,
				EventTime = entity.Event?.EventTime ?? DateTime.MinValue,
				EndDateTime = entity.Event?.EndDateTime,
				ClubId = entity.Event?.ClubId ?? 0,
				ClubName = entity.Event?.Club?.ClubName,
				RegistrationStatus = entity.RegistrationStatus,
				CheckInTime = entity.CheckInTime
			};
		}

		private static EventRegistrationDetailResponse MapToDetailResponse(EventRegistration entity)
		{
			return new EventRegistrationDetailResponse
			{
				EventRegistrationId = entity.EventRegistrationId,
				EventId = entity.EventId,
				UserId = entity.UserId,
				EventName = entity.Event?.EventName,
				EventDescription = entity.Event?.Description,
				EventTime = entity.Event?.EventTime ?? DateTime.MinValue,
				EndDateTime = entity.Event?.EndDateTime,
				EventLocation = entity.Event?.Location,
				EventStatus = entity.Event?.Status,
				ClubId = entity.Event?.ClubId ?? 0,
				ClubName = entity.Event?.Club?.ClubName,
				RegistrationStatus = entity.RegistrationStatus,
				CheckInTime = entity.CheckInTime
			};
		}

		private static AdminEventRegistrationItemResponse MapToAdminItemResponse(EventRegistration entity)
		{
			return new AdminEventRegistrationItemResponse
			{
				EventRegistrationId = entity.EventRegistrationId,
				EventId = entity.EventId,
				EventName = entity.Event?.EventName,
				UserId = entity.UserId,
				FullName = entity.User?.FullName,
				Email = entity.User?.Email,
				Phone = entity.User?.Phone,
				RegistrationStatus = entity.RegistrationStatus,
				CheckInTime = entity.CheckInTime
			};
		}

		private static AdminEventRegistrationDetailResponse MapToAdminDetailResponse(EventRegistration entity)
		{
			return new AdminEventRegistrationDetailResponse
			{
				EventRegistrationId = entity.EventRegistrationId,
				EventId = entity.EventId,
				EventName = entity.Event?.EventName,
				EventDescription = entity.Event?.Description,
				EventTime = entity.Event?.EventTime ?? DateTime.MinValue,
				EndDateTime = entity.Event?.EndDateTime,
				EventLocation = entity.Event?.Location,
				EventStatus = entity.Event?.Status,
				ClubId = entity.Event?.ClubId ?? 0,
				ClubName = entity.Event?.Club?.ClubName,
				UserId = entity.UserId,
				FullName = entity.User?.FullName,
				Email = entity.User?.Email,
				Phone = entity.User?.Phone,
				RegistrationStatus = entity.RegistrationStatus,
				CheckInTime = entity.CheckInTime
			};
		}

		private async Task EnsureEventHasCapacityAsync(Event ev)
		{
			if (!ev.MaxParticipants.HasValue)
			{
				return;
			}

			var maxParticipants = ev.MaxParticipants.Value;
			if (maxParticipants <= 0)
			{
				throw new InvalidOperationException("Sức chứa sự kiện không hợp lệ.");
			}

			var registeredCount = await _eventRegistrationRepository.CountRegisteredByEventAsync(ev.EventId);
			if (registeredCount >= maxParticipants)
			{
				throw new InvalidOperationException("Sự kiện đã đủ số lượng người tham gia.");
			}
		}

		private static byte[] BuildRegistrationsCsv(List<EventRegistration> registrations)
		{
			var builder = new StringBuilder();
			builder.AppendLine("EventRegistrationId,EventId,EventName,UserId,FullName,Email,Phone,RegistrationStatus,CheckInTime");

			foreach (var item in registrations)
			{
				builder.AppendLine(string.Join(",",
					EscapeCsv(item.EventRegistrationId.ToString()),
					EscapeCsv(item.EventId.ToString()),
					EscapeCsv(item.Event?.EventName),
					EscapeCsv(item.UserId.ToString()),
					EscapeCsv(item.User?.FullName),
					EscapeCsv(item.User?.Email),
					EscapeCsv(item.User?.Phone),
					EscapeCsv(item.RegistrationStatus),
					EscapeCsv(item.CheckInTime?.ToString("yyyy-MM-dd HH:mm:ss"))));
			}

			var csvContent = builder.ToString();
			return Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csvContent)).ToArray();
		}

		private static string EscapeCsv(string? value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return string.Empty;
			}

			if (value.Contains('"') || value.Contains(',') || value.Contains('\n') || value.Contains('\r'))
			{
				return $"\"{value.Replace("\"", "\"\"")}\"";
			}

			return value;
		}
	}
}
