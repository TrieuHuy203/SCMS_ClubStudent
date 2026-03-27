using SCMS.Contracts.Interfaces.iRepositores;
using SCMS.DomainEntities.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SCMS.Contracts.DTOs.Requests;

namespace SCMS.DAL.Repositories
{
    // Triển khai các phương thức thao tác với Club trong database
    public class ClubRepository : IClubRepository
    {
        private readonly SCMSDbContext _context;

        // Inject DbContext qua constructor
        public ClubRepository(SCMSDbContext context)
        {
            _context = context;
        }

        // Lấy tất cả các club
        public async Task<IEnumerable<Club>> GetAllClubsAsync()
        {
            return await _context.Clubs.ToListAsync();
        }

        // Lấy club theo Id
        public async Task<Club> GetClubByIdAsync(int id)
        {
            return await _context.Clubs.FindAsync(id);
        }

        // Tạo mới club
        public async Task<Club> CreateClubAsync(Club club)
        {
            _context.Clubs.Add(club);
            await _context.SaveChangesAsync();
            return club;
        }

        // Cập nhật club
        public async Task UpdateClubAsync(Club club)
        {
            _context.Clubs.Update(club);
            await _context.SaveChangesAsync();
        }

        // Xóa club
        public async Task DeleteClubAsync(int id)
        {
            var club = await _context.Clubs.FindAsync(id);
            if (club != null)
            {
                _context.Clubs.Remove(club);
                await _context.SaveChangesAsync();
            }

            
            
        }
        // Lấy club theo trang (pagination), trả về danh sách club và tổng số club (để client biết có bao nhiêu trang)
                public async Task<(List<Club> Clubs, int TotalCount)> GetClubsPagedAsync(int page, int pageSize)
        {
            var query = _context.Clubs.Where(c => c.Status != "Deleted"); // chỉ lấy club chưa xóa
            var totalCount = await query.CountAsync();
            var clubs = await query
                .OrderBy(c => c.ClubId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (clubs, totalCount);
        }

 // Tìm kiếm & phân trang club
        public async Task<(List<Club> Clubs, int TotalCount)> SearchClubsPagedAsync(ClubSearchRequest request)
        {
            var query = _context.Clubs.AsQueryable();

            // Lọc theo trạng thái (nếu có)
            if (!string.IsNullOrWhiteSpace(request.Status))
                query = query.Where(c => c.Status == request.Status);

            // Lọc theo lĩnh vực (nếu có)
            if (!string.IsNullOrWhiteSpace(request.Field))
                query = query.Where(c => c.Field == request.Field);

            // Lọc theo trường (nếu có)
            if (!string.IsNullOrWhiteSpace(request.School))
                query = query.Where(c => c.School == request.School);

            // Tìm kiếm theo từ khóa (tên, mô tả)
            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                var keyword = request.Keyword.Trim().ToLower();
                query = query.Where(c =>
                    c.ClubName.ToLower().Contains(keyword) ||
                    (c.Description != null && c.Description.ToLower().Contains(keyword))
                );
            }

            var totalCount = await query.CountAsync();

            var clubs = await query
                .OrderBy(c => c.ClubId)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return (clubs, totalCount);
        }

    }
}