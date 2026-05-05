using System.Threading.Tasks;
using System.Collections.Generic;
using SCMS.DomainEntities.Entities;

public interface IMembershipRepository
{
    Task<Membership> AddAsync(Membership membership); // Thêm mới
    Task<Membership> UpdateAsync(Membership membership); // Cập nhật membership
    Task<Membership?> GetByUserAndClubAsync(int userId, int clubId); // Kiểm tra đã đăng ký chưa
    Task<User?> GetByIdAsync(int userId);

    // Lấy danh sách đơn của user (có lọc trạng thái + phân trang)
    // Trả về:
    // - Items: danh sách Membership theo trang hiện tại
    // - TotalCount: tổng số bản ghi thỏa điều kiện (để tính tổng số trang)
    Task<(List<Membership> Items, int TotalCount)> GetMyApplicationsAsync(
        int userId,
        string? status,
        int page,
        int pageSize);

    // Lấy chi tiết 1 đơn theo membershipId + userId
    // Dùng userId để đảm bảo user chỉ xem được đơn của chính mình
    Task<Membership?> GetMyApplicationDetailAsync(int membershipId, int userId);

    // Lấy danh sách CLB user đã tham gia (status = Approved), hỗ trợ tìm theo tên CLB + phân trang
    Task<(List<Membership> Items, int TotalCount)> GetMyJoinedClubsAsync(
        int userId,
        string? keyword,
        int page,
        int pageSize);

    // Lấy danh sách thành viên đã được duyệt của một CLB (hỗ trợ tìm kiếm + phân trang)
    Task<(List<Membership> Items, int TotalCount)> GetApprovedMembersByClubAsync(
        int clubId,
        string? keyword,
        int page,
        int pageSize);

    // Lấy membership đã được duyệt của user tại một CLB (dùng cho chức năng rời CLB)
    Task<Membership?> GetApprovedMembershipAsync(int userId, int clubId);

    // Lấy danh sách ClubId mà user đã có membership Approved
    Task<List<int>> GetApprovedClubIdsByUserAsync(int userId);

    // Đồng bộ lại MemberCount của CLB theo số membership Approved thực tế
    Task SyncClubMemberCountAsync(int clubId);
}