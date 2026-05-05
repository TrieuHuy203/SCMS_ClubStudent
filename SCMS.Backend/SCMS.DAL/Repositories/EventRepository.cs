// SCMS.DAL/Repositories/EventRepository.cs

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SCMS.Contracts.Interfaces.iRepositores;
using SCMS.DomainEntities.Entities;

namespace SCMS.DAL.Repositories
{
    /// <summary>
    /// Triển khai các phương thức thao tác dữ liệu cho bảng Event.
    /// </summary>
    public class EventRepository : IEventRepository
    {
        private readonly SCMSDbContext _context;

        public EventRepository(SCMSDbContext context)
        {
            _context = context;
        }


// Thêm mới event
        public async Task<Event> AddAsync(Event entity)
        {
            await _context.Events.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

// Cập nhật thông tin event
        public async Task UpdateAsync(Event entity)
        {
            _context.Events.Update(entity);
            await _context.SaveChangesAsync();
        }

// Xóa event theo Id
        public async Task DeleteAsync(int eventId)
        {
            var entity = await _context.Events.FindAsync(eventId);
            if (entity != null && !entity.IsDeleted)
            {
                entity.IsDeleted = true;
                _context.Events.Update(entity);
                await _context.SaveChangesAsync();
            }
        }


// Lấy event theo Id
    public async Task<Event?> GetByIdAsync(int eventId)
        {
            return await _context.Events
    .Include(e => e.Club)
    .FirstOrDefaultAsync(e => e.EventId == eventId && !e.IsDeleted);
        }

// Lấy danh sách event có phân trang và tìm kiếm
    public async Task<(IEnumerable<Event> Items, int TotalCount)> GetListAsync(string? keyword = null, int? clubId = null, int page = 1, int pageSize = 10)
        {
            var query = _context.Events
    .Include(e => e.Club) // nếu cần lấy thêm thông tin CLB
    .Where(e => !e.IsDeleted)
    .AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(e => e.EventName.Contains(keyword));

            if (clubId.HasValue)
                query = query.Where(e => e.ClubId == clubId.Value);

            var totalCount = await query.CountAsync();

            // Phân trang
            query = query.OrderByDescending(e => e.EventTime)
                         .Skip((page - 1) * pageSize)
                         .Take(pageSize);

            return (await query.ToListAsync(), totalCount);
        }

        public async Task<(IEnumerable<Event> Items, int TotalCount)> GetListByClubIdsAsync(IEnumerable<int> clubIds, string? keyword = null, int page = 1, int pageSize = 10)
        {
            var normalizedClubIds = clubIds?.Distinct().ToList() ?? new List<int>();
            if (normalizedClubIds.Count == 0)
            {
                return (new List<Event>(), 0);
            }

			var query = _context.Events
				.Include(e => e.Club)
				.Where(e => !e.IsDeleted && normalizedClubIds.Contains(e.ClubId) && e.Status == "Approved")
				.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(e => e.EventName.Contains(keyword));
            }

            var totalCount = await query.CountAsync();

            query = query.OrderByDescending(e => e.EventTime)
                         .Skip((page - 1) * pageSize)
                         .Take(pageSize);

            return (await query.ToListAsync(), totalCount);
        }
    }
}