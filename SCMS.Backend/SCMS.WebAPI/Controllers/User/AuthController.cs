using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.Interfaces.iService;

namespace SCMS.WebAPI.Controllers.User
{
    [ApiController]
    //POST http://<host>:<port>/api/user/auth/register
    [Route("api/user/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserCreateRequest request)
        {
            var result = await _userService.CreateUserAsync(request, GetClientIpAddress(), GetDeviceInfo());
            if (!string.IsNullOrEmpty(result.Message) && result.Message != "Tạo user thành công")
                return BadRequest(result);

            return Ok(result);
        }

// Endpoint đăng nhập
// POST http://<host>:<port>/api/auth/login
        [HttpPost("login")]
            public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
            {
                var result = await _userService.LoginAsync(request, GetClientIpAddress(), GetDeviceInfo());
                if (!string.IsNullOrEmpty(result.Message) && result.Message != "Đăng nhập thành công")
                    return BadRequest(result);

                return Ok(result);
            }
    
    // Endpoint xác nhận email
    // GET http://<host>:<port>/api/auth/confirm-email?email=xxx&token=yyy
[HttpGet("confirm-email")]
public async Task<IActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token)
{
    var result = await _userService.ConfirmEmailAsync(email, token);
    if (result)
        return Ok("Xác thực email thành công! Bạn có thể đăng nhập.");
    else
        return BadRequest("Xác thực email thất bại hoặc mã xác nhận không hợp lệ.");
}

        // Endpoint quên mật khẩu
        // POST http://<host>:<port>/api/user/auth/forgot-password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] AuthForgotPasswordRequest request)
        {
            try
            {
                var message = await _userService.ForgotPasswordAsync(request);
                return Ok(new { message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // Endpoint đặt lại mật khẩu
        // POST http://<host>:<port>/api/user/auth/reset-password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] AuthResetPasswordRequest request)
        {
            try
            {
                var message = await _userService.ResetPasswordAsync(request);
                return Ok(new { message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // Endpoint đăng xuất (JWT stateless)
        // POST http://<host>:<port>/api/user/auth/logout
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Không xác định được user đăng nhập." });

            var message = await _userService.LogoutAsync(userId, GetClientIpAddress(), GetDeviceInfo());
            return Ok(new { message });
        }

        // Ưu tiên lấy IP thật từ X-Forwarded-For khi chạy sau reverse proxy.
        private string? GetClientIpAddress()
        {
            var xForwardedFor = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(xForwardedFor))
                return xForwardedFor.Split(',')[0].Trim();

            return HttpContext.Connection.RemoteIpAddress?.ToString();
        }

        // User-Agent giúp phân tích thiết bị đã thực hiện hành động auth.
        private string? GetDeviceInfo()
        {
            return HttpContext.Request.Headers["User-Agent"].FirstOrDefault();
        }
    
    }
}