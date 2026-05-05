using System.Collections.Generic;
using System.Threading.Tasks;
using SCMS.DomainEntities.Entities;

namespace SCMS.Contracts.Interfaces.iRepositores
{
	// Interface thao tác dữ liệu yêu thích CLB
	public interface IClubFavoriteRepository
	{
		// Kiểm tra user đã yêu thích CLB này chưa
		Task<ClubFavorite?> GetByUserAndClubAsync(int userId, int clubId);

		// Lấy chi tiết 1 bản ghi favorite theo id và userId
		Task<ClubFavorite?> GetByIdAsync(int clubFavoriteId, int userId);

		// Thêm mới favorite
		Task<ClubFavorite> AddAsync(ClubFavorite clubFavorite);

		// Xóa favorite
		Task DeleteAsync(ClubFavorite clubFavorite);

		// Lấy danh sách CLB đã yêu thích của user, có hỗ trợ tìm kiếm và phân trang
		Task<(List<ClubFavorite> Items, int TotalCount)> GetFavoritesAsync(
			int userId,
			string? keyword,
			int page,
			int pageSize);

		// Admin xem tất cả favorite, có tìm kiếm và phân trang
		Task<(List<ClubFavorite> Items, int TotalCount)> GetAllFavoritesAsync(
			string? keyword,
			int page,
			int pageSize);

		// Admin xem chi tiết một bản ghi favorite theo id
		Task<ClubFavorite?> GetDetailByIdAsync(int clubFavoriteId);
	}
}
