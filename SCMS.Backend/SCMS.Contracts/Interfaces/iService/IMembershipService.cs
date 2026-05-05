using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.DTOs.Responses;

namespace SCMS.Contracts.Interfaces.iService
{
    public interface IMembershipService
    {
        // Đăng ký tham gia CLB
        Task<MembershipResponse> RegisterAsync(CreateMembershipRequest request);

        // Đăng ký tham gia CLB với userId (lấy từ token)
        Task<MembershipResponse> RegisterAsync(CreateMembershipRequest request, int userId);

        // User xem danh sách đơn của chính mình (có lọc trạng thái và phân trang)
        Task<PagedResult<MembershipResponse>> GetMyApplicationsAsync(
            int userId,
            string? status,
            int page,
            int pageSize);

        // User xem chi tiết 1 đơn của chính mình
        // Trả null nếu không tồn tại hoặc không thuộc user hiện tại
        Task<MembershipResponse?> GetMyApplicationDetailAsync(int membershipId, int userId);

        // User hủy đơn đăng ký của chính mình (chỉ cho phép khi đơn còn Pending)
        Task<MembershipResponse> CancelMyApplicationAsync(int membershipId, int userId);

        // User xem danh sách CLB đã tham gia (đơn đã được duyệt)
        Task<PagedResult<MembershipResponse>> GetMyJoinedClubsAsync(
            int userId,
            string? keyword,
            int page,
            int pageSize);

        // User xem danh sách thành viên của CLB mình đã tham gia
        Task<PagedResult<ClubMemberResponse>> GetMyClubMembersAsync(
            int userId,
            int clubId,
            string? keyword,
            int page,
            int pageSize);

        // User rời CLB đã tham gia (chỉ cho phép khi membership đang Approved)
        Task<MembershipResponse> LeaveClubAsync(int clubId, int userId);
    }
}