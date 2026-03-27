using Microsoft.AspNetCore.Mvc;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.Interfaces.iService;

namespace SCMS.WebAPI.Controllers.User
{
    [ApiController]
    //POST http://<host>:<port>/api/auth/register
    [Route("api/[controller]")]
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
            var result = await _userService.CreateUserAsync(request);
            if (!string.IsNullOrEmpty(result.Message) && result.Message != "Tạo user thành công")
                return BadRequest(result);

            return Ok(result);
        }

// Endpoint đăng nhập
// POST http://<host>:<port>/api/auth/login
        [HttpPost("login")]
            public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
            {
                var result = await _userService.LoginAsync(request);
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
    
    }
}