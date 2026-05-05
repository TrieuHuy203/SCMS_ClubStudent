using System.Collections.Generic;
using System.Threading.Tasks;
using SCMS.DomainEntities.Entities;

namespace SCMS.Contracts.Interfaces.iRepositores
{
	public interface IEventRegistrationRepository
	{
		// Thêm đăng ký tham gia sự kiện mới
		Task<EventRegistration> AddAsync(EventRegistration entity);

		// Cập nhật trạng thái đăng ký (ví dụ: Cancelled)
		Task<EventRegistration> UpdateAsync(EventRegistration entity);

		// Lấy event theo id để kiểm tra event tồn tại/trạng thái
		Task<Event?> GetEventByIdAsync(int eventId);

		// Kiểm tra user đã đăng ký event này chưa
		Task<EventRegistration?> GetByUserAndEventAsync(int userId, int eventId);

		// Lấy 1 registration theo id và user (đảm bảo ownership)
		Task<EventRegistration?> GetByIdAsync(int eventRegistrationId, int userId);

		// Admin lấy chi tiết 1 registration theo id
		Task<EventRegistration?> GetByIdForAdminAsync(int eventRegistrationId);

		// Kiểm tra user có membership Approved trong club không
		Task<bool> IsUserApprovedMemberOfClubAsync(int userId, int clubId);

		// Lấy danh sách đăng ký sự kiện của user (search + paging)
		Task<(List<EventRegistration> Items, int TotalCount)> GetMyRegistrationsAsync(
			int userId,
			string? keyword,
			string? registrationStatus,
			int page,
			int pageSize);

		// Admin xem danh sách đăng ký theo từng sự kiện (search + paging)
		Task<(List<EventRegistration> Items, int TotalCount)> GetRegistrationsByEventAsync(
			int eventId,
			string? keyword,
			string? registrationStatus,
			int page,
			int pageSize);

		// Admin xem toàn bộ danh sách đăng ký sự kiện (search + paging)
		Task<(List<EventRegistration> Items, int TotalCount)> GetAllRegistrationsAsync(
			string? keyword,
			string? registrationStatus,
			int page,
			int pageSize);

		// Đếm số lượng đăng ký đang ở trạng thái Registered của event.
		Task<int> CountRegisteredByEventAsync(int eventId);

		// Lấy toàn bộ danh sách đăng ký theo event để export.
		Task<List<EventRegistration>> GetRegistrationsForExportByEventAsync(int eventId);

		// Đồng bộ ParticipantCount của Event theo số đăng ký đang Registered
		Task SyncEventParticipantCountAsync(int eventId);
	}
}
