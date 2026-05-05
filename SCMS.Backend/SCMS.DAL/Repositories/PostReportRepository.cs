// Repository triển khai thao tác dữ liệu cho PostReport
using SCMS.Contracts.Interfaces.iRepositores;
using SCMS.DomainEntities.Entities;
using SCMS.Contracts.DTOs.Requests;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SCMS.DAL.Repositories
{
    public class PostReportRepository : IPostReportRepository
    {
        private readonly SCMSDbContext _context;

        public PostReportRepository(SCMSDbContext context)
        {
            _context = context;
        }

        // Lấy danh sách report theo điều kiện tìm kiếm/phân trang
        public async Task<List<PostReport>> SearchAsync(PostReportSearchRequest request)
        {
            var query = _context.PostReports.AsQueryable();

            if (!string.IsNullOrEmpty(request.Status))
                query = query.Where(r => r.Status == request.Status);

            if (request.PostId.HasValue)
                query = query.Where(r => r.PostId == request.PostId.Value);

            if (request.UserId.HasValue)
                query = query.Where(r => r.UserId == request.UserId.Value);

            // Phân trang
            int skip = (request.Page - 1) * request.PageSize;
            return await query
                .OrderByDescending(r => r.CreatedAt)
                .Skip(skip)
                .Take(request.PageSize)
                .ToListAsync();
        }

        // Đếm tổng số report theo điều kiện tìm kiếm
        public async Task<int> CountAsync(PostReportSearchRequest request)
        {
            var query = _context.PostReports.AsQueryable();

            if (!string.IsNullOrEmpty(request.Status))
                query = query.Where(r => r.Status == request.Status);

            if (request.PostId.HasValue)
                query = query.Where(r => r.PostId == request.PostId.Value);

            if (request.UserId.HasValue)
                query = query.Where(r => r.UserId == request.UserId.Value);

            return await query.CountAsync();
        }

        // Lấy chi tiết một report theo Id
        public async Task<PostReport?> GetByIdAsync(int postReportId)
        {
            return await _context.PostReports
                .FirstOrDefaultAsync(r => r.PostReportId == postReportId);
        }

        // Cập nhật trạng thái report
        public async Task UpdateStatusAsync(int postReportId, string status)
        {
            var report = await _context.PostReports.FindAsync(postReportId);
            if (report != null)
            {
                report.Status = status;
                await _context.SaveChangesAsync();
            }
        }
    }
}