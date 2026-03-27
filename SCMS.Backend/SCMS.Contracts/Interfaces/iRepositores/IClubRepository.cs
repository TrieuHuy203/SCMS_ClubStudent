using SCMS.DomainEntities.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;


namespace SCMS.Contracts.Interfaces.iRepositores
{
    // Interface định nghĩa các phương thức thao tác với Club trong database
    public interface IClubRepository
    {
        // Lấy tất cả các club
        Task<IEnumerable<Club>> GetAllClubsAsync();

        // Lấy club theo Id
        Task<Club> GetClubByIdAsync(int id);

        // Tạo mới club
        Task<Club> CreateClubAsync(Club club);

        // Cập nhật club
        Task UpdateClubAsync(Club club);

        // Xóa club
        Task DeleteClubAsync(int id);
        // Lấy club theo trang (pagination), trả về danh sách club và tổng số club (để client biết có bao nhiêu trang)
        Task<(List<Club> Clubs, int TotalCount)> GetClubsPagedAsync(int page, int pageSize);
     // Tìm kiếm & phân trang club
        Task<(List<Club> Clubs, int TotalCount)> SearchClubsPagedAsync(ClubSearchRequest request);
    
    }
}