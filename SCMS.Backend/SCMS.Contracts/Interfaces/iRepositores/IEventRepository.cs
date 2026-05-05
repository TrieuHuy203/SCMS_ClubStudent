// SCMS.Contracts/Interfaces/iRepositores/IEventRepository.cs

using System.Collections.Generic;
using System.Threading.Tasks;
using SCMS.DomainEntities.Entities;

namespace SCMS.Contracts.Interfaces.iRepositores
{
    /// <summary>
    /// Interface định nghĩa các phương thức thao tác dữ liệu cho bảng Event.
    /// </summary>
    public interface IEventRepository
    {
        /// <summary>
        /// Thêm mới một sự kiện.
        /// </summary>
        Task<Event> AddAsync(Event entity);

        /// <summary>
        /// Cập nhật thông tin sự kiện.
        /// </summary>
        Task UpdateAsync(Event entity);

        /// <summary>
        /// Xóa mềm sự kiện theo Id (đánh dấu IsDeleted = true).
        /// </summary>
        Task DeleteAsync(int eventId);

        /// <summary>
        /// Lấy chi tiết sự kiện theo Id.
        /// </summary>
        Task<Event?> GetByIdAsync(int eventId);

        /// <summary>
        /// Lấy danh sách sự kiện (có thể phân trang, filter).
        /// </summary>
        Task<(IEnumerable<Event> Items, int TotalCount)> GetListAsync(string? keyword = null, int? clubId = null, int page = 1, int pageSize = 10);

        /// <summary>
        /// Lấy danh sách sự kiện theo tập clubIds (có thể phân trang, filter keyword).
        /// </summary>
        Task<(IEnumerable<Event> Items, int TotalCount)> GetListByClubIdsAsync(IEnumerable<int> clubIds, string? keyword = null, int page = 1, int pageSize = 10);
    }
}