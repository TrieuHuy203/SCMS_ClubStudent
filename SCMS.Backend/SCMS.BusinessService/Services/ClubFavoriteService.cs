using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.DTOs.Responses;
using SCMS.Contracts.Interfaces.iRepositores;
using SCMS.Contracts.Interfaces.iService;
using SCMS.DomainEntities.Entities;

namespace SCMS.BusinessService.Services
{
	// Triển khai nghiệp vụ yêu thích CLB
	public class ClubFavoriteService : IClubFavoriteService
	{
		private readonly IClubFavoriteRepository _clubFavoriteRepository;

		public ClubFavoriteService(IClubFavoriteRepository clubFavoriteRepository)
		{
			_clubFavoriteRepository = clubFavoriteRepository;
		}

		// User yêu thích một CLB
		public async Task<ClubFavoriteResponse> AddFavoriteAsync(int userId, int clubId)
		{
			var existed = await _clubFavoriteRepository.GetByUserAndClubAsync(userId, clubId);
			if (existed != null)
			{
				return MapToResponse(existed);
			}

			var favorite = new ClubFavorite
			{
				UserId = userId,
				ClubId = clubId,
				CreatedAt = DateTime.UtcNow
			};

			var created = await _clubFavoriteRepository.AddAsync(favorite);
			var result = await _clubFavoriteRepository.GetByIdAsync(created.ClubFavoriteId, userId);

			return MapToResponse(result ?? created);
		}

		// User hủy yêu thích một CLB
		public async Task<ClubFavoriteResponse> RemoveFavoriteAsync(int userId, int clubId)
		{
			var existed = await _clubFavoriteRepository.GetByUserAndClubAsync(userId, clubId);
			if (existed == null)
			{
				throw new KeyNotFoundException("Không tìm thấy CLB đã yêu thích để hủy.");
			}

			await _clubFavoriteRepository.DeleteAsync(existed);
			return MapToResponse(existed);
		}

		// User xem danh sách CLB đã yêu thích, có search và phân trang
		public async Task<PagedResult<ClubFavoriteResponse>> GetMyFavoritesAsync(
			int userId,
			ClubFavoriteSearchRequest request)
		{
			var page = request.Page < 1 ? 1 : request.Page;
			var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

			var (items, totalCount) = await _clubFavoriteRepository.GetFavoritesAsync(
				userId,
				request.Keyword,
				page,
				pageSize);

			var responses = items.Select(MapToResponse).ToList();

			return new PagedResult<ClubFavoriteResponse>
			{
				Items = responses,
				TotalCount = totalCount,
				Page = page,
				PageSize = pageSize
			};
		}

		// Kiểm tra user đã yêu thích CLB hay chưa
		public async Task<bool> IsFavoriteAsync(int userId, int clubId)
		{
			var existed = await _clubFavoriteRepository.GetByUserAndClubAsync(userId, clubId);
			return existed != null;
		}

		// Admin xem tất cả favorite, có search và phân trang
		public async Task<PagedResult<ClubFavoriteResponse>> GetAllFavoritesAsync(ClubFavoriteSearchRequest request)
		{
			request ??= new ClubFavoriteSearchRequest();
			var page = request.Page < 1 ? 1 : request.Page;
			var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

			var (items, totalCount) = await _clubFavoriteRepository.GetAllFavoritesAsync(
				request.Keyword,
				page,
				pageSize);

			return new PagedResult<ClubFavoriteResponse>
			{
				Items = items.Select(MapToResponse).ToList(),
				TotalCount = totalCount,
				Page = page,
				PageSize = pageSize
			};
		}

		// Admin xem chi tiết một bản ghi favorite
		public async Task<ClubFavoriteResponse?> GetFavoriteDetailAsync(int clubFavoriteId)
		{
			var item = await _clubFavoriteRepository.GetDetailByIdAsync(clubFavoriteId);
			if (item == null)
			{
				return null;
			}

			return MapToResponse(item);
		}

		// Map entity sang DTO response
		private static ClubFavoriteResponse MapToResponse(ClubFavorite? favorite)
		{
			if (favorite == null)
			{
				return new ClubFavoriteResponse();
			}

			return new ClubFavoriteResponse
			{
				ClubFavoriteId = favorite.ClubFavoriteId,
				UserId = favorite.UserId,
				Username = favorite.User?.Username,
				FullName = favorite.User?.FullName,
				Email = favorite.User?.Email,
				ClubId = favorite.ClubId,
				ClubName = favorite.Club?.ClubName ?? string.Empty,
				Description = favorite.Club?.Description,
				Field = favorite.Club?.Field,
				School = favorite.Club?.School,
				MemberCount = favorite.Club?.MemberCount,
				CreatedAt = favorite.CreatedAt
			};
		}
	}
}
