// Triển khai các hàm thao tác với bảng User ở tầng Repository
using System.Threading.Tasks;
using SCMS.DomainEntities.Entities;
using SCMS.Contracts.Interfaces.iRepositores;
using Microsoft.EntityFrameworkCore;

namespace SCMS.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SCMSDbContext _context; // DbContext để thao tác với database

        // Inject DbContext qua constructor
        public UserRepository(SCMSDbContext context)
        {
            _context = context;
        }
        // Kiểm tra xem username đã tồn tại trong database chưa
        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }
        // Kiểm tra xem email đã tồn tại trong database chưa
        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }
        
        // Lấy tất cả user từ database  
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .ToListAsync();
        }


        // Thêm user mới vào database
        public async Task<User> AddUserAsync(User user)
        {
            // Thêm user vào DbSet
            _context.Users.Add(user);

            // Lưu thay đổi vào database
            await _context.SaveChangesAsync();

            // Trả về user vừa thêm (có UserId)
            return user;
        }

        // Lấy user theo Id
          public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        // Cập nhật user
        public async Task<User> UpdateUserAsync(User user)
        {
            _context.Users.Update(user); // Cập nhật user trong DbSet
            await _context.SaveChangesAsync();
            return user;
        }

        // Xóa user theo Id
        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId); // Tìm user theo Id
            if (user == null)
                return false;
            user.IsDisabled = true; // Đánh dấu user là vô hiệu hóa thay vì xóa hẳn
            _context.Users.Update(user); // Cập nhật user trong DbSet
            await _context.SaveChangesAsync();
            return true;
        }

// Vô hiệu hóa user theo Id
        public async Task<bool> SetUserDisabledAsync(int userId, bool isDisabled)
        {
            var user = await _context.Users.FindAsync(userId); //.  
            if (user == null)
                return false;
            user.IsDisabled = isDisabled; // Cập nhật trạng thái vô hiệu hóa
            _context.Users.Update(user); // Cập nhật user trong DbSet
            await _context.SaveChangesAsync(); // Lưu thay đổi vào database
            return true; // Trả về true nếu thành công
        }

         /// <summary>
        /// Tìm kiếm user theo keyword (username, email, fullname), status, isDisabled.
        /// Nếu tham số null sẽ bỏ qua điều kiện đó.
        /// </summary>
        public async Task<List<User>> SearchUsersAsync(string keyword, string status, bool? isDisabled)
        {
            var query = _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .AsQueryable();

            // Lọc theo keyword (username, email, fullname)
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(u =>
                    u.Username.Contains(keyword) ||
                    u.Email.Contains(keyword) ||
                    u.FullName.Contains(keyword)
                );
            }

            // Lọc theo status
            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(u => u.Status == status);
            }

            // Lọc theo isDisabled
            if (isDisabled.HasValue)
            {
                query = query.Where(u => u.IsDisabled == isDisabled.Value);
            }

            return await query.ToListAsync();
        }
// phân trang, lấy user theo trang (pagination), trả về danh sách user và tổng số user (để client biết có bao nhiêu trang)
public async Task<(List<User> Users, int TotalCount)> GetUsersPagedAsync(int page, int pageSize)
{
    var query = _context.Users.AsQueryable();
    var totalCount = await query.CountAsync();
    var users = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
    return (users, totalCount);
}
// Hàm tìm kiếm user theo keyword, status, isDisabled và phân trang, trả về danh sách user và tổng số user
public async Task<(List<User> Users, int TotalCount)> SearchUsersPagedAsync(string keyword, string status, bool? isDisabled, int page, int pageSize)
{
    var query = _context.Users.AsQueryable();

    if (!string.IsNullOrEmpty(keyword))
        query = query.Where(u => u.Username.Contains(keyword) || u.FullName.Contains(keyword) || u.Email.Contains(keyword));

    if (!string.IsNullOrEmpty(status))
        query = query.Where(u => u.Status == status);

    if (isDisabled.HasValue)
        query = query.Where(u => u.IsDisabled == isDisabled);

    var totalCount = await query.CountAsync();
    var users = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    return (users, totalCount);

}

// Hàm lấy user theo username hoặc email (dùng cho đăng nhập)
public async Task<User> GetUserByUsernameOrEmailAsync(string usernameOrEmail)
{
    return await _context.Users
        .FirstOrDefaultAsync(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail);
}

    // Lấy user theo token xác thực email
    public async Task<User?> GetUserByEmailConfirmationTokenAsync(string token)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.EmailConfirmationToken == token);
    }

    // Lấy user theo email
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    
    }
}