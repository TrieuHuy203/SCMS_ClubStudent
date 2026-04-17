using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.Interfaces.iService;

namespace SCMS.WebAPI.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/event-registrations")]
    public class AdminEventRegistrationController : ControllerBase
    {
        private readonly IEventRegistrationService _eventRegistrationService;

        public AdminEventRegistrationController(IEventRegistrationService eventRegistrationService)
        {
            _eventRegistrationService = eventRegistrationService;
        }

        // Admin xem chi tiết một đăng ký sự kiện
        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetRegistrationDetail(int id)
        {
            try
            {
                var result = await _eventRegistrationService.GetRegistrationDetailForAdminAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // Admin xem toàn bộ danh sách đăng ký sự kiện (list + paging)
        [HttpGet("list")]
        public async Task<IActionResult> GetAllRegistrationsList([FromQuery] EventRegistrationSearchRequest request)
        {
            try
            {
                var result = await _eventRegistrationService.GetAllRegistrationsAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // Giữ endpoint cũ để tương thích ngược
        [HttpGet("registrations")]
        public async Task<IActionResult> GetAllRegistrations([FromQuery] EventRegistrationSearchRequest request)
        {
            try
            {
                var result = await _eventRegistrationService.GetAllRegistrationsAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // Admin tìm kiếm toàn bộ danh sách đăng ký (search + paging)
        [HttpGet("search")]
        public async Task<IActionResult> SearchAllRegistrations([FromQuery] EventRegistrationSearchRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Keyword))
            {
                return BadRequest(new { message = "Vui lòng nhập keyword để tìm kiếm." });
            }

            try
            {
                var result = await _eventRegistrationService.GetAllRegistrationsAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // Admin xem danh sách người đăng ký theo từng sự kiện (list + paging)
        [HttpGet("events/{eventId}/registrations/list")]
        public async Task<IActionResult> GetRegistrationsByEventList(
            int eventId,
            [FromQuery] EventRegistrationSearchRequest request)
        {
            try
            {
                var result = await _eventRegistrationService.GetRegistrationsByEventAsync(eventId, request);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // Giữ endpoint cũ để tương thích ngược
        [HttpGet("events/{eventId}/registrations")] 
        public async Task<IActionResult> GetRegistrationsByEvent(
            int eventId,
            [FromQuery] EventRegistrationSearchRequest request)
        {
            try
            {
                var result = await _eventRegistrationService.GetRegistrationsByEventAsync(eventId, request);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // Admin tìm kiếm người đăng ký theo từng sự kiện (search + paging)
        [HttpGet("events/{eventId}/registrations/search")]
        public async Task<IActionResult> SearchRegistrationsByEvent(
            int eventId,
            [FromQuery] EventRegistrationSearchRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Keyword))
            {
                return BadRequest(new { message = "Vui lòng nhập keyword để tìm kiếm." });
            }

            try
            {
                var result = await _eventRegistrationService.GetRegistrationsByEventAsync(eventId, request);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // Admin export danh sách người đăng ký theo sự kiện (CSV)
        [HttpGet("events/{eventId}/registrations/export")]
        public async Task<IActionResult> ExportRegistrationsByEvent(int eventId)
        {
            try
            {
                var fileBytes = await _eventRegistrationService.ExportRegistrationsByEventForAdminAsync(eventId);
                var fileName = $"event-{eventId}-registrations-{DateTime.Now:yyyyMMddHHmmss}.csv";
                return File(fileBytes, "text/csv", fileName);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}