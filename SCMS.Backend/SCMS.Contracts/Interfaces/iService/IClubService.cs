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
        Task<ClubResponse> CreateClubAsync(ClubCreateRequest request);

        // Cập nhật club
        Task UpdateClubAsync(ClubUpdateRequest request);

        // Xóa club
        Task DeleteClubAsync(int id);

        // Lấy club theo trang (pagination), trả về danh sách club và tổng số club (để client biết có bao nhiêu trang)
        Task<PagedResult<ClubResponse>> GetClubsPagedAsync(int page, int pageSize);
        // Tìm kiếm & phân trang club
         Task<PagedResult<ClubResponse>> SearchClubsPagedAsync(ClubSearchRequest request);
    }
}


