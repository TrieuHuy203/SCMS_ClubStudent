// SCMS.WebAPI/Controllers/Admin/EventController.cs

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests.Event;
using SCMS.Contracts.Interfaces.iService;

namespace SCMS.WebAPI.Controllers.Admin 

    
{
    [ApiController]
    [Route("api/admin/events")]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        /// <summary>
        /// Tạo mới sự kiện.
        /// </summary>
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateEventRequest request)
        {
            try
            {
                if (!TryGetCurrentUserId(out var adminId, out var unauthorizedResult))
                {
                    return unauthorizedResult!;
                }

                var eventId = await _eventService.CreateEventAsync(request, adminId);
                return Ok(new
                {
                    message = "Tạo sự kiện thành công.",
                    eventId
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = $"Tạo sự kiện thất bại: {ex.Message}" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = $"Tạo sự kiện thất bại: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Tạo sự kiện thất bại: {ex.Message}" });
            }
        }

        /// <summary>
        /// Cập nhật sự kiện.
        /// </summary>
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEventRequest request)
        {
            if (id != request.EventId)
            {
                return BadRequest(new { message = "Cập nhật sự kiện thất bại: id trên URL không khớp với EventId trong body." });
            }

            try
            {
                if (!TryGetCurrentUserId(out var adminId, out var unauthorizedResult))
                {
                    return unauthorizedResult!;
                }

                await _eventService.UpdateEventAsync(request, adminId);
                return Ok(new { message = "Cập nhật sự kiện thành công." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = $"Cập nhật sự kiện thất bại: {ex.Message}" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = $"Cập nhật sự kiện thất bại: {ex.Message}" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = $"Cập nhật sự kiện thất bại: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Cập nhật sự kiện thất bại: {ex.Message}" });
            }
        }

        /// <summary>
        /// Xóa sự kiện.
        /// </summary>
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (!TryGetCurrentUserId(out var adminId, out var unauthorizedResult))
                {
                    return unauthorizedResult!;
                }

                await _eventService.DeleteEventAsync(id, adminId);
                return Ok(new { message = "Xóa sự kiện thành công." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = $"Xóa sự kiện thất bại: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Xóa sự kiện thất bại: {ex.Message}" });
            }
        }

        /// <summary>
        /// Duyệt sự kiện.
        /// </summary>
        [HttpPost("approve/{id}")]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                if (!TryGetCurrentUserId(out var adminId, out var unauthorizedResult))
                {
                    return unauthorizedResult!;
                }

                await _eventService.ApproveEventAsync(id, adminId);
                return Ok(new { message = "Duyệt sự kiện thành công." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = $"Duyệt sự kiện thất bại: {ex.Message}" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = $"Duyệt sự kiện thất bại: {ex.Message}" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = $"Duyệt sự kiện thất bại: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Duyệt sự kiện thất bại: {ex.Message}" });
            }
        }

        /// <summary>
        /// Từ chối sự kiện và lưu lý do.
        /// </summary>
        [HttpPost("reject/{id}")]
        public async Task<IActionResult> Reject(int id, [FromBody] RejectEventRequest request)
        {
            try
            {
                if (!TryGetCurrentUserId(out var adminId, out var unauthorizedResult))
                {
                    return unauthorizedResult!;
                }

                await _eventService.RejectEventAsync(id, adminId, request.RejectReason);
                return Ok(new { message = "Từ chối sự kiện thành công." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = $"Từ chối sự kiện thất bại: {ex.Message}" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = $"Từ chối sự kiện thất bại: {ex.Message}" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = $"Từ chối sự kiện thất bại: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Từ chối sự kiện thất bại: {ex.Message}" });
            }
        }

        /// <summary>
        /// Lấy chi tiết sự kiện.
        /// </summary>
        [HttpGet("detail/{id}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var result = await _eventService.GetEventDetailAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách sự kiện (có filter, phân trang).
        /// </summary>
        [HttpGet("list")]
        public async Task<IActionResult> GetList([FromQuery] string? keyword, [FromQuery] int? clubId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _eventService.GetEventListAsync(keyword, clubId, page, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Tìm kiếm sự kiện theo từ khóa.
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword, [FromQuery] int? clubId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _eventService.SearchEventAsync(keyword, clubId, page, pageSize);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = $"Tìm kiếm sự kiện thất bại: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Tìm kiếm sự kiện thất bại: {ex.Message}" });
            }
        }

        private bool TryGetCurrentUserId(out int userId, out IActionResult? unauthorizedResult)
        {
            unauthorizedResult = null;
            userId = 0;

            var userIdClaim = User.FindFirst("UserId") ?? User.FindFirst("userId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out userId))
            {
                unauthorizedResult = Unauthorized(new { message = "Không tìm thấy UserId hợp lệ trong token!" });
                return false;
            }

            return true;
        }
            /// <summary>
        /// Lấy danh sách sự kiện chờ duyệt (Pending).
        /// </summary>
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingEvents([FromQuery] int? clubId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _eventService.GetEventListAsync(null, clubId, page, pageSize);
            // Lọc trạng thái chờ duyệt
            result.Items = result.Items.FindAll(e => e.Status == "Pending");
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách sự kiện đã duyệt (Approved).
        /// </summary>
        [HttpGet("approved")]
        public async Task<IActionResult> GetApprovedEvents([FromQuery] int? clubId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _eventService.GetEventListAsync(null, clubId, page, pageSize);
            // Lọc trạng thái đã duyệt
            result.Items = result.Items.FindAll(e => e.Status == "Approved");
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách sự kiện đã từ chối (Rejected).
        /// </summary>
        [HttpGet("rejected")]
        public async Task<IActionResult> GetRejectedEvents([FromQuery] int? clubId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _eventService.GetEventListAsync(null, clubId, page, pageSize);
            // Lọc trạng thái đã từ chối
            result.Items = result.Items.FindAll(e => e.Status == "Rejected");
            return Ok(result);
        }
    }
}