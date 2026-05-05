using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SCMS.Contracts.Interfaces.iRepositores;
using SCMS.DomainEntities.Entities;

namespace SCMS.DAL.Repositories
{
	// Triển khai thao tác dữ liệu yêu thích CLB
	public class ClubFavoriteRepository : IClubFavoriteRepository
	{
		private readonly SCMSDbContext _context;

		public ClubFavoriteRepository(SCMSDbContext context)
		{
			_context = context;
		}

		// Kiểm tra user đã yêu thích CLB này chưa
		public async Task<ClubFavorite?> GetByUserAndClubAsync(int userId, int clubId)
		{
			return await _context.ClubFavorites
				.Include(cf => cf.Club)
				.Include(cf => cf.User)
				.FirstOrDefaultAsync(cf => cf.UserId == userId && cf.ClubId == clubId);
		}

		// Lấy chi tiết 1 favorite theo id và chủ sở hữu
		public async Task<ClubFavorite?> GetByIdAsync(int clubFavoriteId, int userId)
		{
			return await _context.ClubFavorites
				.Include(cf => cf.Club)
				.Include(cf => cf.User)
				.FirstOrDefaultAsync(cf => cf.ClubFavoriteId == clubFavoriteId && cf.UserId == userId);
		}

		// Thêm mới favorite
		public async Task<ClubFavorite> AddAsync(ClubFavorite clubFavorite)
		{
			_context.ClubFavorites.Add(clubFavorite);
			await _context.SaveChangesAsync();
			return clubFavorite;
		}

		// Xóa favorite
		public async Task DeleteAsync(ClubFavorite clubFavorite)
		{
			_context.ClubFavorites.Remove(clubFavorite);
			await _context.SaveChangesAsync();
		}

		// Lấy danh sách favorite của user, có search và phân trang
		public async Task<(List<ClubFavorite> Items, int TotalCount)> GetFavoritesAsync(
			int userId,
			string? keyword,
			int page,
			int pageSize)
		{
			if (page < 1) page = 1;
			if (pageSize < 1) pageSize = 10;

			var query = _context.ClubFavorites
				.AsNoTracking()
				.Include(cf => cf.Club)
				.Where(cf => cf.UserId == userId);

			if (!string.IsNullOrWhiteSpace(keyword))
			{
				var normalizedKeyword = keyword.Trim().ToLower();
				query = query.Where(cf =>
					cf.Club.ClubName.ToLower().Contains(normalizedKeyword) ||
					(cf.Club.Description != null && cf.Club.Description.ToLower().Contains(normalizedKeyword)));
			}

			var totalCount = await query.CountAsync();

			var items = await query
				.OrderByDescending(cf => cf.CreatedAt)
				.ThenByDescending(cf => cf.ClubFavoriteId)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			return (items, totalCount);
		}

		// Admin xem tất cả favorite, có search và phân trang
		public async Task<(List<ClubFavorite> Items, int TotalCount)> GetAllFavoritesAsync(
			string? keyword,
			int page,
			int pageSize)
		{
			if (page < 1) page = 1;
			if (pageSize < 1) pageSize = 10;

			var query = _context.ClubFavorites
				.AsNoTracking()
				.Include(cf => cf.Club)
				.Include(cf => cf.User)
				.AsQueryable();

			if (!string.IsNullOrWhiteSpace(keyword))
			{
				var normalizedKeyword = keyword.Trim().ToLower();
				query = query.Where(cf =>
					(cf.Club.ClubName != null && cf.Club.ClubName.ToLower().Contains(normalizedKeyword)) ||
					(cf.Club.Description != null && cf.Club.Description.ToLower().Contains(normalizedKeyword)) ||
					(cf.User.Username != null && cf.User.Username.ToLower().Contains(normalizedKeyword)) ||
					(cf.User.FullName != null && cf.User.FullName.ToLower().Contains(normalizedKeyword)) ||
					(cf.User.Email != null && cf.User.Email.ToLower().Contains(normalizedKeyword)));
			}

			var totalCount = await query.CountAsync();

			var items = await query
				.OrderByDescending(cf => cf.CreatedAt)
				.ThenByDescending(cf => cf.ClubFavoriteId)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			return (items, totalCount);
		}

		// Admin xem chi tiết một bản ghi favorite theo id
		public async Task<ClubFavorite?> GetDetailByIdAsync(int clubFavoriteId)
		{
			return await _context.ClubFavorites
				.AsNoTracking()
				.Include(cf => cf.Club)
				.Include(cf => cf.User)
				.FirstOrDefaultAsync(cf => cf.ClubFavoriteId == clubFavoriteId);
		}
	}
}
