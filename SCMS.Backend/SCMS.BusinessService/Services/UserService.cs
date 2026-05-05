using System;
using System.Linq; 
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.DTOs.Responses;
using SCMS.Contracts.Interfaces.iService;
using SCMS.Contracts.Interfaces.iRepositores;
using SCMS.DomainEntities.Entities;
using BCrypt.Net;  // thư viện mã hoá pass BCrypt  
using Microsoft.Extensions.Configuration; // để đọc cấu hình appsettings
using MailKit.Net.Smtp; // thư viện gửi email MailKit
using MimeKit; // thư viện tạo nội dung email MimeKit
 


namespace SCMS.BusinessService.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthLogService _authLogService;
        private readonly IAuditLogService _auditLogService;
        private readonly IConfiguration _configuration;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRoleRepository _userRoleRepository;
      

        public UserService(IUserRepository userRepository, IAuthLogService authLogService, IAuditLogService auditLogService, IConfiguration configuration,  IRoleRepository roleRepository,IUserRoleRepository userRoleRepository)
        {
            _userRepository = userRepository;
            _authLogService = authLogService;
            _auditLogService = auditLogService;
            _configuration = configuration;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
        }
        // Hàm lấy tất cả user
        public async Task<List<User>> GetAllUsersAsync() // Trả về danh sách User (có thể map sang DTO nếu cần)
        {
            return await _userRepository.GetAllUsersAsync(); // Gọi repository để lấy dữ liệu từ DB
        }


        // Hàm tạo user mới
        public async Task<UserCreateResponse> CreateUserAsync(UserCreateRequest request, string? ipAddress = null, string? deviceInfo = null)
        {
            // 1. Validate dữ liệu đầu vào
            if (string.IsNullOrWhiteSpace(request.Username) || request.Username.Length < 4) // username tối thiểu 4 ký tự
                return new UserCreateResponse { Message = "Username không được rỗng và phải >= 4 ký tự" };

            if (string.IsNullOrWhiteSpace(request.Password) || !IsStrongPassword(request.Password))
                return new UserCreateResponse { Message = "Password không hợp lệ (ít nhất 6 ký tự, có chữ hoa, số, ký tự đặc biệt...)" };

            if (string.IsNullOrWhiteSpace(request.FullName))
                return new UserCreateResponse { Message = "FullName không được rỗng" };

            if (string.IsNullOrWhiteSpace(request.Email) || !IsValidEmail(request.Email))
                return new UserCreateResponse { Message = "Email không hợp lệ" };

            // 2. Kiểm tra trùng username/email
            if (await _userRepository.UsernameExistsAsync(request.Username))
                return new UserCreateResponse { Message = "Username đã tồn tại" };

            if (await _userRepository.EmailExistsAsync(request.Email))
                return new UserCreateResponse { Message = "Email đã tồn tại" };

            // 3. Hash password
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
// 4. Sinh mã xác thực email và hạn sử dụng
string emailConfirmationToken = Guid.NewGuid().ToString();
DateTime emailConfirmationTokenExpiry = DateTime.UtcNow.AddHours(24); // Token có hiệu lực 24h


         
// 5. Tạo entity User
var user = new User
{
    Username = request.Username,
    PasswordHash = passwordHash,
    FullName = request.FullName,
    Email = request.Email,
    Phone = request.Phone,
    Status = "Active",
    CreatedAt = DateTime.Now,
    IsDisabled = false,
    IsEmailConfirmed = false,
    EmailConfirmationToken = emailConfirmationToken,
    EmailConfirmationTokenExpiry = emailConfirmationTokenExpiry
};

            // 5. Lưu user vào DB
            var createdUser = await _userRepository.AddUserAsync(user);
            // 5.1 Gán role mặc định "User"
            var role = await _roleRepository.GetByNameAsync("User");

            if (role == null)
            {
                throw new Exception("Role 'User' không tồn tại");
            }

            var userRole = new UserRole
            {
                UserId = createdUser.UserId,
                RoleId = role.RoleId
            };

            await _userRoleRepository.AddAsync(userRole);

            // Ghi log đăng ký thành công (best effort, không làm fail luồng chính).
            await SafeWriteAuthLogAsync(createdUser.UserId, "REGISTER", "SUCCESS", ipAddress, deviceInfo);

            // Gửi email xác thực
await SendConfirmationEmailAsync(user.Email, user.EmailConfirmationToken);

            // 6. Trả về response
            return new UserCreateResponse
            {
                UserId = createdUser.UserId,
                Username = createdUser.Username,
                Email = createdUser.Email,
                Message = "Tạo user thành công"
            };

            
        }

        // Hàm cập nhật thông tin user
        public async Task<UserUpdateResponse> UpdateUserAsync(int userId, int currentUserId, UserUpdateRequest request)
        {
            // 1. Lấy user từ DB
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return new UserUpdateResponse { Message = "User không tồn tại" };
            }
            // Nếu email có thay đổi thì kiểm tra trùng email
            if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
            {
                if (await _userRepository.EmailExistsAsync(request.Email))
                    return new UserUpdateResponse { Message = "Email đã tồn tại" };
                user.Email = request.Email;
            }
            // 2. Validate dữ liệu đầu vào (ví dụ: email hợp lệ, fullname không rỗng)
            if (string.IsNullOrWhiteSpace(request.FullName))
                return new UserUpdateResponse { Message = "FullName không được rỗng" };

            if (string.IsNullOrWhiteSpace(request.Email) || !IsValidEmail(request.Email))
                return new UserUpdateResponse { Message = "Email không hợp lệ" };

            // 3. Cập nhật thông tin user
            user.FullName = request.FullName;
            user.Email = request.Email;
            user.Phone = request.Phone;
            user.Status = request.Status;

            // Nếu admin gửi IsEmailConfirmed thì cập nhật luôn
            if (request.IsEmailConfirmed.HasValue)
            {
                user.IsEmailConfirmed = request.IsEmailConfirmed.Value;
            }

            // 4. Lưu thay đổi vào DB
            var updatedUser = await _userRepository.UpdateUserAsync(user);

            // Ghi audit log với actor là người thực hiện thao tác (admin hiện tại).
            await SafeWriteAuditLogAsync(currentUserId, "User", user.UserId, "UPDATE");

            // 5. Trả về response
            return new UserUpdateResponse
            {
                UserId = updatedUser.UserId,
                Username = updatedUser.Username,
                FullName = updatedUser.FullName,
                Email = updatedUser.Email,
                Phone = updatedUser.Phone,
                Status = updatedUser.Status,
                Message = "Cập nhật user thành công"
            };
        }

        // Hàm xóa user (thực chất là vô hiệu hóa)
        public async Task<bool> DeleteUserAsync(int userId, int currentUserId)
        {
            if (userId == currentUserId)
            return false; // Không cho xoá chính mình
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return false;
            var deleted = await _userRepository.DeleteUserAsync(userId);

            if (deleted)
            {
                // Actor là currentUserId, bản ghi bị tác động là userId.
                await SafeWriteAuditLogAsync(currentUserId, "User", userId, "DELETE");
            }

            return deleted;
        }

        // Hàm vô hiệu hóa user
    // Hàm vô hiệu hóa / mở khóa user
public async Task<bool> SetUserDisabledAsync(int userId, bool isDisabled, int currentUserId)
{
    // Không cho tự khóa hoặc tự mở khóa chính mình
    if (userId == currentUserId)
        return false;

    // Lấy user để kiểm tra tồn tại
    var user = await _userRepository.GetByIdAsync(userId);
    if (user == null)
        return false;

    // Business rule:
    // isDisabled = true  => Status = Inactive
    // isDisabled = false => Status = Active
    user.IsDisabled = isDisabled;
    user.Status = isDisabled ? "Inactive" : "Active";

    // Lưu thay đổi xuống DB
    await _userRepository.UpdateUserAsync(user);

    // Ghi audit log bật/tắt tài khoản user.
    await SafeWriteAuditLogAsync(currentUserId, "User", userId, isDisabled ? "DISABLE" : "ENABLE");
    return true;
}

        // Hàm lấy chi tiết user theo Id
        public async Task<UserDetailResponse> GetUserDetailAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return null;

            return new UserDetailResponse
            {
                Id = user.UserId,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Status = user.Status,
                AvatarUrl = user.AvatarUrl,
                IsDisabled = user.IsDisabled ?? false,
                CreatedAt = user.CreatedAt ?? DateTime.MinValue,
                Roles = user.UserRoles?.Select(ur => ur.Role?.RoleName).Where(r => r != null).ToList() ?? new List<string>()
            };
        }

        // Hàm lấy hồ sơ cá nhân của chính user đang đăng nhập
        public async Task<UserDetailResponse> GetMyProfileAsync(int userId)
        {
            // Tái sử dụng logic lấy user theo Id để tránh duplicate code
            return await GetUserDetailAsync(userId);
        }

        // Hàm cập nhật hồ sơ cá nhân của chính user đang đăng nhập
        public async Task<UserDetailResponse> UpdateMyProfileAsync(int userId, UserProfileUpdateRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User không tồn tại");

            // Validate dữ liệu cơ bản của profile
            if (string.IsNullOrWhiteSpace(request.FullName))
                throw new ArgumentException("FullName không được để trống.");

            if (string.IsNullOrWhiteSpace(request.Email) || !IsValidEmail(request.Email))
                throw new ArgumentException("Email không hợp lệ.");

            // Chỉ kiểm tra trùng email khi user thực sự đổi email
            if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
            {
                if (await _userRepository.EmailExistsAsync(request.Email))
                    throw new InvalidOperationException("Email đã tồn tại.");
            }

            // Cập nhật hồ sơ cá nhân, không ảnh hưởng các dữ liệu nghiệp vụ khác
            user.FullName = request.FullName;
            user.Email = request.Email;
            user.Phone = request.Phone;
            user.AvatarUrl = request.AvatarUrl;

            await _userRepository.UpdateUserAsync(user);

            // User tự cập nhật hồ sơ cá nhân.
            await SafeWriteAuditLogAsync(userId, "User", userId, "UPDATE_PROFILE");

            // Trả về hồ sơ mới nhất để client render lại ngay sau khi lưu
            return await GetUserDetailAsync(userId);
        }



        // Hàm đổi mật khẩu của chính user đang đăng nhập
        public async Task<string> ChangeMyPasswordAsync(int userId, UserChangePasswordRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User không tồn tại");

            // Kiểm tra dữ liệu bắt buộc
            if (string.IsNullOrWhiteSpace(request.CurrentPassword) ||
                string.IsNullOrWhiteSpace(request.NewPassword) ||
                string.IsNullOrWhiteSpace(request.ConfirmPassword))
            {
                throw new ArgumentException("Vui lòng nhập đầy đủ thông tin.");
            }

            // Xác thực mật khẩu hiện tại
            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
                throw new InvalidOperationException("Mật khẩu hiện tại không đúng.");

            // Kiểm tra mật khẩu mới
            if (!string.Equals(request.NewPassword, request.ConfirmPassword, StringComparison.Ordinal))
                throw new ArgumentException("Mật khẩu xác nhận không khớp.");

            if (!IsStrongPassword(request.NewPassword))
                throw new ArgumentException("Mật khẩu mới không đủ mạnh.");

            // Hash mật khẩu mới và lưu xuống DB
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _userRepository.UpdateUserAsync(user);

            await SafeWriteAuditLogAsync(userId, "User", userId, "CHANGE_PASSWORD");

            return "Đổi mật khẩu thành công.";
        }

        // Hàm admin reset mật khẩu cho user mục tiêu
        public async Task<string> AdminResetUserPasswordAsync(int adminId, int targetUserId, AdminResetPasswordRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (adminId == targetUserId)
                throw new InvalidOperationException("Admin không thể dùng API reset cho chính mình. Hãy dùng API đổi mật khẩu.");

            var targetUser = await _userRepository.GetByIdAsync(targetUserId);
            if (targetUser == null)
                throw new KeyNotFoundException("User cần reset mật khẩu không tồn tại.");

            if (string.IsNullOrWhiteSpace(request.NewPassword) || string.IsNullOrWhiteSpace(request.ConfirmPassword))
                throw new ArgumentException("Vui lòng nhập đầy đủ mật khẩu mới và xác nhận mật khẩu.");

            if (!string.Equals(request.NewPassword, request.ConfirmPassword, StringComparison.Ordinal))
                throw new ArgumentException("Mật khẩu xác nhận không khớp.");

            if (!IsStrongPassword(request.NewPassword))
                throw new ArgumentException("Mật khẩu mới không đủ mạnh.");

            targetUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _userRepository.UpdateUserAsync(targetUser);

            await SafeWriteAuditLogAsync(adminId, "User", targetUserId, "ADMIN_RESET_PWD");

            return "Admin reset mật khẩu cho user thành công.";
        }

        // Hàm kiểm tra email hợp lệ
        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        // Hàm kiểm tra password mạnh (tối thiểu 6 ký tự, có chữ hoa, số, ký tự đặc biệt)
        private bool IsStrongPassword(string password)
        {
            if (password.Length < 6) return false;
            if (!Regex.IsMatch(password, @"[A-Z]")) return false;
            if (!Regex.IsMatch(password, @"[0-9]")) return false;
            if (!Regex.IsMatch(password, @"[\W_]")) return false;
            return true;
        }
  public async Task<IEnumerable<UserAvatarDto>> GetAllUserAvatarsAsync()
    {
        return await _userRepository.GetAllUserAvatarsAsync();
    }

    public async Task<UserAvatarDto?> GetUserAvatarAsync(int userId)
    {
        return await _userRepository.GetUserAvatarAsync(userId);
    }

    public async Task<bool> DeleteUserAvatarAsync(int userId)
    {
        return await _userRepository.DeleteUserAvatarAsync(userId);
    }
         /// <summary>
        /// Tìm kiếm user theo keyword, status, isDisabled (nâng cao).
        /// </summary>
        public async Task<List<UserDetailResponse>> SearchUsersAsync(string keyword, string status, bool? isDisabled)
        {
            var users = await _userRepository.SearchUsersAsync(keyword, status, isDisabled);

            // Map entity User sang DTO UserDetailResponse
            return users.Select(user => new UserDetailResponse
            {
                Id = user.UserId,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Status = user.Status,
                AvatarUrl = user.AvatarUrl,
                IsDisabled = user.IsDisabled ?? false,
                CreatedAt = user.CreatedAt ?? DateTime.MinValue,
                Roles = user.UserRoles?.Select(ur => ur.Role?.RoleName).Where(r => r != null).ToList() ?? new List<string>()
            }).ToList();
        }


// Hàm lấy user theo trang (pagination), trả về danh sách DTO response và tổng số user
public async Task<PagedResult<UserDetailResponse>> GetUsersPagedAsync(int page, int pageSize)
{
    var (users, totalCount) = await _userRepository.GetUsersPagedAsync(page, pageSize);
    var userDtos = users.Select(u => new UserDetailResponse
    {
        Id = u.UserId,
        Username = u.Username,
        FullName = u.FullName,
        Email = u.Email,
        Phone = u.Phone,
        Status = u.Status,
        AvatarUrl = u.AvatarUrl,
        IsDisabled = u.IsDisabled ?? false,
        CreatedAt = u.CreatedAt ?? DateTime.MinValue,
        Roles = u.UserRoles?.Select(ur => ur.Role?.RoleName).Where(r => r != null).ToList() ?? new List<string>()
    }).ToList();
    return new PagedResult<UserDetailResponse>
    {
        Items = userDtos,
        TotalCount = totalCount,
        Page = page,
        PageSize = pageSize
    };
}


    public async Task<PagedResult<UserDetailResponse>> SearchUsersPagedAsync(UserSearchRequest request)
{
    var (users, totalCount) = await _userRepository.SearchUsersPagedAsync(
        request.Keyword, request.Status, request.IsDisabled, request.Page, request.PageSize);

    var userDtos = users.Select(u => new UserDetailResponse
    {
        Id = u.UserId,
        Username = u.Username,
        FullName = u.FullName,
        Email = u.Email,
        Phone = u.Phone,
        Status = u.Status,
        AvatarUrl = u.AvatarUrl,
        IsDisabled = u.IsDisabled ?? false,
        CreatedAt = u.CreatedAt ?? DateTime.MinValue,
        Roles = u.UserRoles?.Select(ur => ur.Role?.RoleName).Where(r => r != null).ToList() ?? new List<string>()
    }).ToList();

    return new PagedResult<UserDetailResponse>
    {
        Items = userDtos,
        TotalCount = totalCount,
        Page = request.Page,
        PageSize = request.PageSize
    };
}



// Hàm đăng nhập
public async Task<UserLoginResponse> LoginAsync(UserLoginRequest request, string? ipAddress = null, string? deviceInfo = null)
{
    // 1. Lấy user theo username hoặc email
    var user = await _userRepository.GetUserByUsernameOrEmailAsync(request.UsernameOrEmail);
    if (user == null)
    {
        // Không có userId để lưu vào bảng AuthLog nên bỏ qua thất bại kiểu này.
        return new UserLoginResponse { Message = "Tài khoản hoặc mật khẩu không đúng" };
    }

    // 2. Kiểm tra password (dùng BCrypt)
    bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
    if (!isPasswordValid)
    {
        // Ghi nhận đăng nhập thất bại để phục vụ audit bảo mật.
        await SafeWriteAuthLogAsync(user.UserId, "LOGIN", "FAILED", ipAddress, deviceInfo);
        return new UserLoginResponse { Message = "Tài khoản hoặc mật khẩu không đúng" };
    }

    // 3. Kiểm tra tài khoản bị vô hiệu hóa
    if (user.IsDisabled ?? false)
    {
        await SafeWriteAuthLogAsync(user.UserId, "LOGIN", "FAILED", ipAddress, deviceInfo);
        return new UserLoginResponse { Message = "Tài khoản đã bị vô hiệu hóa. Vui lòng liên hệ quản trị viên." };
    }

    // 4. Kiểm tra trạng thái tài khoản
    if (!string.Equals(user.Status, "Active", StringComparison.OrdinalIgnoreCase))
    {
        await SafeWriteAuthLogAsync(user.UserId, "LOGIN", "FAILED", ipAddress, deviceInfo);
        return new UserLoginResponse { Message = "Tài khoản không ở trạng thái hoạt động." };
    }

    // 5. Kiểm tra xác thực email
    if (!user.IsEmailConfirmed)
    {
        await SafeWriteAuthLogAsync(user.UserId, "LOGIN", "FAILED", ipAddress, deviceInfo);
        return new UserLoginResponse { Message = "Tài khoản chưa xác thực email. Vui lòng kiểm tra email để xác nhận." };
    }

    // 6. Sinh JWT token
    string token = GenerateJwtToken(user);

    // Chỉ log SUCCESS khi đăng nhập hoàn tất và đã cấp token.
    await SafeWriteAuthLogAsync(user.UserId, "LOGIN", "SUCCESS", ipAddress, deviceInfo);

    // 7. Trả về response
    return new UserLoginResponse
    {
        UserId = user.UserId,
        Username = user.Username,
        Token = token,
        Message = "Đăng nhập thành công"
    };
}

// Hàm sinh JWT token (ví dụ đơn giản, cần cấu hình lại cho production)
private string GenerateJwtToken(User user)
{
    // Cần cài package: System.IdentityModel.Tokens.Jwt
    var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
    var key = System.Text.Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
    var tokenDescriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
    {
        Subject = new System.Security.Claims.ClaimsIdentity(new[]
        {
            new System.Security.Claims.Claim("UserId", user.UserId.ToString()),
            new System.Security.Claims.Claim("Username", user.Username ?? "")
            // Có thể bổ sung claim Role, Email...
        }),
        Expires = DateTime.UtcNow.AddDays(7),
        SigningCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(
            new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key),
            Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature)
    };
    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
}

// Hàm xác nhận email
 public async Task<bool> ConfirmEmailAsync(string email, string token)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null) return false;
            if (user.EmailConfirmationToken != token) return false;
            if (user.EmailConfirmationTokenExpiry.HasValue && user.EmailConfirmationTokenExpiry < DateTime.UtcNow) return false;

            user.IsEmailConfirmed = true;
            user.EmailConfirmationToken = null;
            user.EmailConfirmationTokenExpiry = null;
            await _userRepository.UpdateUserAsync(user);
            return true;
        }

// Hàm quên mật khẩu: tạo token reset và gửi email hướng dẫn
public async Task<string> ForgotPasswordAsync(AuthForgotPasswordRequest request)
{
    if (request == null || string.IsNullOrWhiteSpace(request.Email))
        throw new ArgumentException("Email không được để trống.");

    // Trả message chung để tránh lộ thông tin email có tồn tại hay không
    const string genericMessage = "Nếu email tồn tại, hệ thống đã gửi hướng dẫn đặt lại mật khẩu.";

    var user = await _userRepository.GetUserByEmailAsync(request.Email);
    if (user == null)
        return genericMessage;

    // Dùng cột riêng cho luồng reset password để không ảnh hưởng xác thực email.
    user.ResetPasswordToken = Guid.NewGuid().ToString();
    user.ResetPasswordTokenExpiry = DateTime.UtcNow.AddMinutes(30);

    await _userRepository.UpdateUserAsync(user);
    await SendResetPasswordEmailAsync(user.Email, user.ResetPasswordToken);

    return genericMessage;
}

// Hàm đặt lại mật khẩu bằng email + token
public async Task<string> ResetPasswordAsync(AuthResetPasswordRequest request)
{
    if (request == null)
        throw new ArgumentException("Dữ liệu không hợp lệ.");

    if (string.IsNullOrWhiteSpace(request.Email) ||
        string.IsNullOrWhiteSpace(request.Token) ||
        string.IsNullOrWhiteSpace(request.NewPassword) ||
        string.IsNullOrWhiteSpace(request.ConfirmPassword))
    {
        throw new ArgumentException("Vui lòng nhập đầy đủ thông tin.");
    }

    if (!string.Equals(request.NewPassword, request.ConfirmPassword, StringComparison.Ordinal))
        throw new ArgumentException("Mật khẩu xác nhận không khớp.");

    if (!IsStrongPassword(request.NewPassword))
        throw new ArgumentException("Mật khẩu mới không đủ mạnh.");

    var user = await _userRepository.GetUserByEmailAsync(request.Email);
    if (user == null)
        throw new InvalidOperationException("Email hoặc token không hợp lệ.");

    if (!string.Equals(user.ResetPasswordToken, request.Token, StringComparison.Ordinal))
        throw new InvalidOperationException("Email hoặc token không hợp lệ.");

    if (!user.ResetPasswordTokenExpiry.HasValue || user.ResetPasswordTokenExpiry.Value < DateTime.UtcNow)
        throw new InvalidOperationException("Token đã hết hạn.");

    // Cập nhật mật khẩu mới và xóa token sau khi dùng thành công
    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
    user.ResetPasswordToken = null;
    user.ResetPasswordTokenExpiry = null;

    await _userRepository.UpdateUserAsync(user);
    return "Đặt lại mật khẩu thành công.";
}

// Logout với JWT stateless: phía client xóa token là đủ
public async Task<string> LogoutAsync(int userId, string? ipAddress = null, string? deviceInfo = null)
{
    // JWT đang stateless nên logout chỉ là thao tác logic phía client + ghi log server.
    await SafeWriteAuthLogAsync(userId, "LOGOUT", "SUCCESS", ipAddress, deviceInfo);
    return "Đăng xuất thành công.";
}
// Hàm lấy user theo email (dùng cho đăng nhập hoặc xác nhận email)
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetUserByEmailAsync(email);
        }

// Hàm gửi email xác thực (có thể gọi sau khi tạo user thành công)

private async Task SendConfirmationEmailAsync(string toEmail, string token)
{
    var message = new MimeMessage();
    message.From.Add(new MailboxAddress("Student Club", "trieuhuy03032003@gmail.com"));
    message.To.Add(new MailboxAddress("", toEmail));
    message.Subject = "Xác thực email tài khoản Student Club";

// tạo link xác thực địa chỉ IP mạng cục LAN
    string confirmationLink = $"http://192.168.1.32:5217/api/user/auth/confirm-email?email={Uri.EscapeDataString(toEmail)}&token={Uri.EscapeDataString(token)}";

    message.Body = new TextPart("plain")
    {
        Text = $"Chào bạn,\n\nVui lòng xác thực email bằng cách nhấn vào link sau:\n{confirmationLink}\n\nHoặc dùng token: {token}\n\nCảm ơn!"
    };

    using var client = new SmtpClient();
    await client.ConnectAsync("smtp.gmail.com", 587, false);
    await client.AuthenticateAsync("trieuhuy03032003@gmail.com", "moatkskeuwzcdqmy"); // Thay bằng email và mật khẩu ứng dụng (app password) của bạn
    try
    {
        await client.SendAsync(message);
        Console.WriteLine("Gửi email xác thực thành công!");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Lỗi gửi email: " + ex.Message);
    }
    await client.DisconnectAsync(true);
}

// Ghi auth log theo cơ chế "best effort" để không chặn luồng chính nếu log bị lỗi.
private async Task SafeWriteAuthLogAsync(int userId, string actionType, string status, string? ipAddress, string? deviceInfo)
{
    try
    {
        await _authLogService.LogAuthAsync(userId, actionType, status, ipAddress, deviceInfo);
    }
    catch
    {
        // Không throw lại để tránh làm fail nghiệp vụ chính khi ghi log gặp lỗi tạm thời.
    }
}

// Ghi audit log theo cơ chế "best effort" để bảo toàn nghiệp vụ chính.
private async Task SafeWriteAuditLogAsync(int actorUserId, string tableName, int recordId, string actionType, string? oldValue = null, string? newValue = null)
{
    try
    {
        await _auditLogService.LogAuditAsync(actorUserId, tableName, recordId, actionType, oldValue, newValue);
    }
    catch
    {
        // Không throw lại để tránh làm fail luồng chính khi ghi audit bị lỗi.
    }
}

// Hàm gửi email reset password
private async Task SendResetPasswordEmailAsync(string toEmail, string token)
{
    var message = new MimeMessage();
    message.From.Add(new MailboxAddress("Student Club", "trieuhuy03032003@gmail.com"));
    message.To.Add(new MailboxAddress("", toEmail));
    message.Subject = "Yêu cầu đặt lại mật khẩu Student Club";


    // Link FE (sau này dùng cho giao diện)
    string resetLinkFE = $"http://localhost:5173/reset-password?email={Uri.EscapeDataString(toEmail)}&token={Uri.EscapeDataString(token)}";
    // Link API backend (cho phép test trực tiếp)
    string resetLinkAPI = $"http://192.168.1.145:5217/api/user/auth/reset-password?email={Uri.EscapeDataString(toEmail)}&token={Uri.EscapeDataString(token)}&newPassword=NhapMatKhauMoi&confirmPassword=NhapMatKhauMoi";

    message.Body = new TextPart("plain")
    {
        Text = $"Chào bạn,\n\nBạn vừa yêu cầu đặt lại mật khẩu.\n" +
               $"1. Đổi mật khẩu qua giao diện web (frontend):\n{resetLinkFE}\n\n" +
               $"2. Đổi mật khẩu trực tiếp qua API (dùng cho test hoặc Postman):\n{resetLinkAPI}\n\n" +
               $"Hoặc copy token: {token}\n\nToken có hiệu lực trong 30 phút."
    };

    using var client = new SmtpClient();
    await client.ConnectAsync("smtp.gmail.com", 587, false);
    await client.AuthenticateAsync("trieuhuy03032003@gmail.com", "moatkskeuwzcdqmy");

    try
    {
        await client.SendAsync(message);
        Console.WriteLine("Gửi email reset mật khẩu thành công!");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Lỗi gửi email reset mật khẩu: " + ex.Message);
    }

    await client.DisconnectAsync(true);
}



}

}