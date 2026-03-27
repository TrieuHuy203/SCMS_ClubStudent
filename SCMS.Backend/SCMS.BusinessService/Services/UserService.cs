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
using MailKit.Net.Smtp; // thư viện gửi email MailKit
using MimeKit; // thư viện tạo nội dung email MimeKit
 


namespace SCMS.BusinessService.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        // Hàm lấy tất cả user
        public async Task<List<User>> GetAllUsersAsync() // Trả về danh sách User (có thể map sang DTO nếu cần)
        {
            return await _userRepository.GetAllUsersAsync(); // Gọi repository để lấy dữ liệu từ DB
        }


        // Hàm tạo user mới
        public async Task<UserCreateResponse> CreateUserAsync(UserCreateRequest request)
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
        public async Task<UserUpdateResponse> UpdateUserAsync(int userId, UserUpdateRequest request)
        {
            // 1. Lấy user từ DB
            var user = await _userRepository.GetUserByIdAsync(userId);
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

            // 4. Lưu thay đổi vào DB
            var updatedUser = await _userRepository.UpdateUserAsync(user);

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
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                return false;
            return await _userRepository.DeleteUserAsync(userId);
        }

        // Hàm vô hiệu hóa user
        public async Task<bool> SetUserDisabledAsync(int userId, bool isDisabled, int currentUserId)
        {
            if (userId == currentUserId)
                return false; // Không cho tự vô hiệu hoá/mở khoá chính mình
            return await _userRepository.SetUserDisabledAsync(userId, isDisabled); // Gọi repository để cập nhật trạng thái vô hiệu hóa
        }


        // Hàm lấy chi tiết user theo Id
        public async Task<UserDetailResponse> GetUserDetailAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
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
                IsDisabled = user.IsDisabled ?? false,
                CreatedAt = user.CreatedAt ?? DateTime.MinValue,
                Roles = user.UserRoles?.Select(ur => ur.Role?.RoleName).Where(r => r != null).ToList() ?? new List<string>()
            };
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
public async Task<UserLoginResponse> LoginAsync(UserLoginRequest request)
{
    // 1. Lấy user theo username hoặc email
    var user = await _userRepository.GetUserByUsernameOrEmailAsync(request.UsernameOrEmail);
    if (user == null)
    {
        return new UserLoginResponse { Message = "Tài khoản hoặc mật khẩu không đúng" };
    }

    // 2. Kiểm tra password (dùng BCrypt)
    bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
    if (!isPasswordValid)
    {
        return new UserLoginResponse { Message = "Tài khoản hoặc mật khẩu không đúng" };
    }

    // 3. Kiểm tra xác thực email
    if (!user.IsEmailConfirmed)
    {
        return new UserLoginResponse { Message = "Tài khoản chưa xác thực email. Vui lòng kiểm tra email để xác nhận." };
    }

    // 4. (Nếu dùng JWT, sinh token ở đây)
    string token = ""; // TODO: Sinh JWT nếu cần

    // 5. Trả về response
    return new UserLoginResponse
    {
        UserId = user.UserId,
        Username = user.Username,
        Token = token,
        Message = "Đăng nhập thành công"
    };
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

    string confirmationLink = $"http://192.168.1.97:5217/api/auth/confirm-email?email={Uri.EscapeDataString(toEmail)}&token={Uri.EscapeDataString(token)}";

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

}
    }
