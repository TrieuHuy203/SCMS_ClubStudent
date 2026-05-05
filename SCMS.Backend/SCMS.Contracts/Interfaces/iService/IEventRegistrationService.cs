using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.DTOs.Responses;

namespace SCMS.Contracts.Interfaces.iService
{
	public interface IEventRegistrationService
	{
		// User đăng ký tham gia sự kiện
		Task<EventRegistrationDetailResponse> RegisterAsync(EventRegisterRequest request, int userId);

		// User xem danh sách các sự kiện đã đăng ký (search + paging)
		Task<PagedResult<EventRegistrationItemResponse>> GetMyRegistrationsAsync(
			int userId,
			EventRegistrationSearchRequest request);

		// User xem chi tiết 1 đăng ký thuộc về chính mình
		Task<EventRegistrationDetailResponse?> GetMyRegistrationDetailAsync(int eventRegistrationId, int userId);

		// User hủy đăng ký sự kiện của chính mình
		Task<EventRegistrationDetailResponse> CancelMyRegistrationAsync(int eventRegistrationId, int userId);

		// Admin xem danh sách người đăng ký theo từng sự kiện
		Task<PagedResult<AdminEventRegistrationItemResponse>> GetRegistrationsByEventAsync(
			int eventId,
			EventRegistrationSearchRequest request);

		// Thành viên CLB (nhóm trưởng/chủ nhiệm ở bước phân quyền sau) xem danh sách người đăng ký theo sự kiện thuộc CLB của mình
		Task<PagedResult<AdminEventRegistrationItemResponse>> GetClubEventRegistrationsForMemberAsync(
			int userId,
			int eventId,
			EventRegistrationSearchRequest request);

		// Admin xem toàn bộ danh sách người đăng ký sự kiện
		Task<PagedResult<AdminEventRegistrationItemResponse>> GetAllRegistrationsAsync(
			EventRegistrationSearchRequest request);

		// Admin xem chi tiết một đăng ký sự kiện theo id
		Task<AdminEventRegistrationDetailResponse> GetRegistrationDetailForAdminAsync(int eventRegistrationId);

		// Admin export danh sách đăng ký theo sự kiện (CSV)
		Task<byte[]> ExportRegistrationsByEventForAdminAsync(int eventId);

		// Thành viên CLB export danh sách đăng ký theo sự kiện thuộc CLB của mình (CSV)
		Task<byte[]> ExportClubEventRegistrationsForMemberAsync(int userId, int eventId);
	}
}
