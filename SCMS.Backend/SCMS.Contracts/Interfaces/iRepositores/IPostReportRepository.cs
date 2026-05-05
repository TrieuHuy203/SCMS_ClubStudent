// Interface repository cho PostReport, định nghĩa các phương thức thao tác dữ liệu báo cáo vi phạm
using SCMS.DomainEntities.Entities;
using SCMS.Contracts.DTOs.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCMS.Contracts.Interfaces.iRepositores
{
    public interface IPostReportRepository
    {
        // Lấy danh sách report theo điều kiện tìm kiếm/phân trang
        Task<List<PostReport>> SearchAsync(PostReportSearchRequest request);

        // Đếm tổng số report theo điều kiện tìm kiếm
        Task<int> CountAsync(PostReportSearchRequest request);

        // Lấy chi tiết một report theo Id
        Task<PostReport?> GetByIdAsync(int postReportId);

        // Cập nhật trạng thái report
        Task UpdateStatusAsync(int postReportId, string status);

        // (Có thể bổ sung thêm các phương thức khác nếu cần)
    }
}