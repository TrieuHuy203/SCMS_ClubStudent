// Interface service cho PostReport, định nghĩa các phương thức nghiệp vụ cho admin
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCMS.Contracts.Interfaces.iService
{
    public interface IPostReportService
    {
        // Lấy danh sách report (có phân trang, lọc)
        Task<(List<PostReportResponse> Items, int TotalCount)> SearchAsync(PostReportSearchRequest request);

        // Lấy chi tiết một report
        Task<PostReportResponse?> GetByIdAsync(int postReportId);

        // Đánh dấu report đã xem
        Task<bool> MarkReviewedAsync(int postReportId, int adminId);

        // Đánh dấu report đã xử lý
        Task<bool> MarkResolvedAsync(int postReportId, int adminId);
    }
}