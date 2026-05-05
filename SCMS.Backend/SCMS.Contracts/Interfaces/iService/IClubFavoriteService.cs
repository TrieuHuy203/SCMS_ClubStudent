using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.DTOs.Responses;

namespace SCMS.Contracts.Interfaces.iService
{
	// Interface nghiệp vụ cho chức năng yêu thích CLB
	public interface IClubFavoriteService
	{
		// User yêu thích một CLB
		Task<ClubFavoriteResponse> AddFavoriteAsync(int userId, int clubId);

		// User hủy yêu thích một CLB
		Task<ClubFavoriteResponse> RemoveFavoriteAsync(int userId, int clubId);

		// User xem danh sách CLB đã yêu thích, có search và phân trang
		Task<PagedResult<ClubFavoriteResponse>> GetMyFavoritesAsync(
			int userId,
			ClubFavoriteSearchRequest request);

		// Kiểm tra user đã yêu thích CLB hay chưa
		Task<bool> IsFavoriteAsync(int userId, int clubId);

		// Admin xem tất cả favorite, có search và phân trang
		Task<PagedResult<ClubFavoriteResponse>> GetAllFavoritesAsync(ClubFavoriteSearchRequest request);

		// Admin xem chi tiết một bản ghi favorite
		Task<ClubFavoriteResponse?> GetFavoriteDetailAsync(int clubFavoriteId);
	}
}
