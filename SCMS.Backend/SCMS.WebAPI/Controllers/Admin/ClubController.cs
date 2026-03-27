using Microsoft.AspNetCore.Mvc;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.DTOs.Responses;
using SCMS.Contracts.Interfaces.iService;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCMS.WebAPI.Controllers.Admin
{
    /// <summary>
    /// Controller quản lý Club (CRUD cho admin)
    /// </summary>
    [ApiController]
    [Route("api/admin/[controller]")]
    public class ClubController : ControllerBase
    {
        private readonly IClubService _clubService;

        // Inject service qua constructor
        public ClubController(IClubService clubService)
        {
            _clubService = clubService;
        }

        /// <summary>
        /// Lấy danh sách tất cả các club
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClubResponse>>> GetAllClubs()
        {
            var clubs = await _clubService.GetAllClubsAsync();
            return Ok(clubs);
        }

        /// <summary>
        /// Lấy thông tin club theo Id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ClubResponse>> GetClubById(int id)
        {
            var club = await _clubService.GetClubByIdAsync(id);
            if (club == null) return NotFound();
            return Ok(club);
        }

        /// <summary>
        /// Tạo mới một club
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ClubResponse>> CreateClub([FromBody] ClubCreateRequest request)
        {
            var created = await _clubService.CreateClubAsync(request);
            return CreatedAtAction(nameof(GetClubById), new { id = created.ClubId }, new {
                message = "Tạo club thành công",
                data = created
            });
        }

        /// <summary>
        /// Cập nhật thông tin club
        /// </summary>
      [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClub(int id, [FromBody] ClubUpdateRequest request)
        {
            if (id != request.ClubId) return BadRequest("Id không khớp với ClubId trong request");
            await _clubService.UpdateClubAsync(request);
            return Ok(new { message = "Cập nhật club thành công" });
        }

        /// <summary>
        /// Xóa club theo Id
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClub(int id)
        {
            await _clubService.DeleteClubAsync(id);
            return NoContent();
        }

        // Lấy club theo trang (pagination), trả về danh sách club và tổng số club (để client biết có bao nhiêu trang)
        [HttpGet("paged")]
            public async Task<IActionResult> GetClubsPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
            {
                var result = await _clubService.GetClubsPagedAsync(page, pageSize);
                return Ok(result); // Trả về PagedResult<ClubResponse>
            }
              /// <summary>
        /// Tìm kiếm & phân trang club (lọc nâng cao)
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> SearchClubs([FromQuery] ClubSearchRequest request)
        {
            var result = await _clubService.SearchClubsPagedAsync(request);
            return Ok(result); // Trả về PagedResult<ClubResponse>
        }
    }
}