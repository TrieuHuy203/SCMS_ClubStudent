using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SCMS.DomainEntities.Entities;
using SCMS.Contracts.Interfaces.iRepositores;

namespace SCMS.DAL.Repositories
{
    public class MembershipRepository : IMembershipRepository
    {
        private readonly SCMSDbContext _context;
        public MembershipRepository(SCMSDbContext context)
        {
            _context = context;
        }

        // Thêm mới membership
        public async Task<Membership> AddAsync(Membership membership)
        {
            _context.Memberships.Add(membership);
            await _context.SaveChangesAsync();
            return membership;
        }

        public async Task<Membership> UpdateAsync(Membership membership)
        {
            _context.Memberships.Update(membership);
            await _context.SaveChangesAsync();
            return membership;
        }

        // Kiểm tra user đã đăng ký CLB này chưa
        public async Task<Membership?> GetByUserAndClubAsync(int userId, int clubId)
        {
            return await _context.Memberships
                .FirstOrDefaultAsync(m => m.UserId == userId && m.ClubId == clubId);
        }
            public async Task<User?> GetByIdAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }
        // Lấy danh sách đơn của user (lọc status + phân trang)
public async Task<(List<Membership> Items, int TotalCount)> GetMyApplicationsAsync(
    int userId,
    string? status,
    int page,
    int pageSize)
{
    // Chuẩn hóa paging để tránh page/pageSize không hợp lệ
    if (page < 1) page = 1;
    if (pageSize < 1) pageSize = 10;

    // Query cơ bản: chỉ lấy đơn thuộc user hiện tại
    var query = _context.Memberships
        .AsNoTracking()
        .Include(m => m.Club) // để service có thể lấy thông tin CLB nếu cần map response
        .Where(m => m.UserId == userId);

    // Nếu có truyền status thì lọc theo status
    if (!string.IsNullOrWhiteSpace(status))
    {
        var normalizedStatus = status.Trim().ToLower();
        query = query.Where(m => m.Status != null && m.Status.ToLower() == normalizedStatus);
    }

    var totalCount = await query.CountAsync();

    var items = await query
        .OrderByDescending(m => m.MembershipId) // đơn mới trước
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    return (items, totalCount);
}

// Lấy chi tiết 1 đơn của user theo membershipId + userId (ownership)
public async Task<Membership?> GetMyApplicationDetailAsync(int membershipId, int userId)
{
    return await _context.Memberships
        .AsNoTracking()
        .Include(m => m.Club)
        .Include(m => m.User)
        .FirstOrDefaultAsync(m => m.MembershipId == membershipId && m.UserId == userId);
}

        public async Task<(List<Membership> Items, int TotalCount)> GetMyJoinedClubsAsync(
            int userId,
            string? keyword,
            int page,
            int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var query = _context.Memberships
                .AsNoTracking()
                .Include(m => m.Club)
                .Include(m => m.User)
                .Where(m => m.UserId == userId && m.Status != null && m.Status.ToLower() == "approved");

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var normalizedKeyword = keyword.Trim().ToLower();
                query = query.Where(m => m.Club.ClubName.ToLower().Contains(normalizedKeyword));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(m => m.JoinedAt)
                .ThenByDescending(m => m.MembershipId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<(List<Membership> Items, int TotalCount)> GetApprovedMembersByClubAsync(
            int clubId,
            string? keyword,
            int page,
            int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var query = _context.Memberships
                .AsNoTracking()
                .Include(m => m.User)
                .Where(m => m.ClubId == clubId && m.Status != null && m.Status.ToLower() == "approved");

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var normalizedKeyword = keyword.Trim().ToLower();
                query = query.Where(m =>
                    m.User.FullName.ToLower().Contains(normalizedKeyword)
                    || m.User.Username.ToLower().Contains(normalizedKeyword));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(m => m.User.FullName)
                .ThenByDescending(m => m.JoinedAt)
                .ThenByDescending(m => m.MembershipId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Membership?> GetApprovedMembershipAsync(int userId, int clubId)
        {
            return await _context.Memberships
                .Include(m => m.Club)
                .Include(m => m.User)
                .FirstOrDefaultAsync(m =>
                    m.UserId == userId
                    && m.ClubId == clubId
                    && m.Status != null
                    && m.Status.ToLower() == "approved");
        }

                public async Task<List<int>> GetApprovedClubIdsByUserAsync(int userId)
                {
                    return await _context.Memberships
                    .AsNoTracking()
                    .Where(m => m.UserId == userId
                            && m.Status != null
                            && m.Status.ToLower() == "approved")
                    .Select(m => m.ClubId)
                    .Distinct()
                    .ToListAsync();
                }

// Đồng bộ lại MemberCount của CLB theo số membership Approved thực tế
        public async Task SyncClubMemberCountAsync(int clubId)
        {
            var approvedCount = await _context.Memberships
                .CountAsync(m =>
                    m.ClubId == clubId
                    && m.Status != null
                    && m.Status.ToLower() == "approved");

            var club = await _context.Clubs.FirstOrDefaultAsync(c => c.ClubId == clubId);
            if (club == null)
            {
                return;
            }

            club.MemberCount = approvedCount;
            await _context.SaveChangesAsync();
        }
    }
}