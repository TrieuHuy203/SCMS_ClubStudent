using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCMS.Contracts.Interfaces.iService
{
    // Interface định nghĩa các phương thức nghiệp vụ cho Club
    public interface IClubService
    {
        // Lấy tất cả các club
        Task<IEnumerable<ClubResponse>> GetAllClubsAsync();

        // Lấy club theo Id
        Task<ClubResponse> GetClubByIdAsync(int id);

        // Tạo mới club
        Task<ClubResponse> CreateClubAsync(ClubCreateRequest request, int actorUserId);

        // User gửi yêu cầu tạo club (trạng thái Pending)
        Task<ClubResponse> SubmitClubRegistrationAsync(ClubCreateRequest request, int userId);

        // Cập nhật club
        Task UpdateClubAsync(ClubUpdateRequest request, int actorUserId);

        // Xóa club
        Task DeleteClubAsync(int id, int actorUserId);

        // Lấy club theo trang (pagination), trả về danh sách club và tổng số club (để client biết có bao nhiêu trang)
        Task<PagedResult<ClubResponse>> GetClubsPagedAsync(int page, int pageSize);
        // Tìm kiếm & phân trang club
         Task<PagedResult<ClubResponse>> SearchClubsPagedAsync(ClubSearchRequest request);

        // Admin lấy danh sách club đang chờ duyệt
        Task<PagedResult<ClubResponse>> GetPendingClubsAsync(int page, int pageSize);

        // Admin duyệt/từ chối yêu cầu tạo club
        Task<ClubResponse> ApproveClubAsync(int clubId, int actorUserId);
        Task<ClubResponse> RejectClubAsync(int clubId, int actorUserId, string rejectReason);
    
    // Bật hoặc tắt hoạt động club
Task<bool> SetClubDisabledAsync(int clubId, bool isDisabled, int actorUserId);
    
    Task<ClubStatisticsDto> GetStatisticsAsync(int clubId); // Lấy thống kê số lượng thành viên, bài viết, sự kiện, bình luận của một club
    }
}


