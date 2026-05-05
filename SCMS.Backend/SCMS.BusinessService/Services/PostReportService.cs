// Service triển khai nghiệp vụ cho PostReport
using SCMS.Contracts.Interfaces.iService;
using SCMS.Contracts.Interfaces.iRepositores;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.DTOs.Responses;
using SCMS.DomainEntities.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace SCMS.BusinessService.Services
{
    public class PostReportService : IPostReportService
    {
        private readonly IPostReportRepository _repository;

        public PostReportService(IPostReportRepository repository)
        {
            _repository = repository;
        }

        // Lấy danh sách report (có phân trang, lọc)
        public async Task<(List<PostReportResponse> Items, int TotalCount)> SearchAsync(PostReportSearchRequest request)
        {
            var reports = await _repository.SearchAsync(request);
            var total = await _repository.CountAsync(request);

            // Map entity sang DTO response
            var items = reports.Select(r => new PostReportResponse
            {
                PostReportId = r.PostReportId,
                PostId = r.PostId,
                UserId = r.UserId,
                Reason = r.Reason,
                Description = r.Description,
                Status = r.Status,
                CreatedAt = r.CreatedAt
            }).ToList();

            return (items, total);
        }

        // Lấy chi tiết một report
        public async Task<PostReportResponse?> GetByIdAsync(int postReportId)
        {
            var r = await _repository.GetByIdAsync(postReportId);
            if (r == null) return null;

            return new PostReportResponse
            {
                PostReportId = r.PostReportId,
                PostId = r.PostId,
                UserId = r.UserId,
                Reason = r.Reason,
                Description = r.Description,
                Status = r.Status,
                CreatedAt = r.CreatedAt
            };
        }

        // Đánh dấu report đã xem
        public async Task<bool> MarkReviewedAsync(int postReportId, int adminId)
        {
            // Có thể kiểm tra quyền adminId ở đây nếu cần
            await _repository.UpdateStatusAsync(postReportId, "Reviewed");
            return true;
        }

        // Đánh dấu report đã xử lý
        public async Task<bool> MarkResolvedAsync(int postReportId, int adminId)
        {
            // Có thể kiểm tra quyền adminId ở đây nếu cần
            await _repository.UpdateStatusAsync(postReportId, "Resolved");
            return true;
        }
    }
}