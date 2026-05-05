using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.Interfaces.iService;
using SCMS.WebAPI.Attributes;
using SCMS.DomainEntities.Enums; // using directive for AppPermission enum
namespace SCMS.WebAPI.Controllers.Admin

{
	[ApiController]
	[Route("api/admin/club-favorites")]
	public class AdminClubFavoriteController : ControllerBase
	{
		private readonly IClubFavoriteService _clubFavoriteService;

		public AdminClubFavoriteController(IClubFavoriteService clubFavoriteService)
		{
			_clubFavoriteService = clubFavoriteService;
		}
        [Permission(AppPermission.Admin_Club_Favorite_View_List)]
		// Admin xem tất cả favorite của user (search + pagination)
		[HttpGet("list")]
		public async Task<IActionResult> GetAll([FromQuery] ClubFavoriteSearchRequest request)
		{
			request ??= new ClubFavoriteSearchRequest();
			if (request.Page < 1) request.Page = 1;
			if (request.PageSize < 1) request.PageSize = 10;

			var result = await _clubFavoriteService.GetAllFavoritesAsync(request);
			return Ok(result);
		}

		[Permission(AppPermission.Admin_Club_Favorite_View_Detail)]
		// Admin xem chi tiết 1 bản ghi favorite
		[HttpGet("detail/{id}")]
		public async Task<IActionResult> GetDetail(int id)
		{
			var result = await _clubFavoriteService.GetFavoriteDetailAsync(id);
			if (result == null)
			{
				return NotFound(new { message = "Không tìm thấy bản ghi yêu thích." });
			}

			return Ok(result);
		}
	}
}
