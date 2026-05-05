
// Interface định nghĩa các hàm thao tác với bảng User ở tầng Repository
using System.Threading.Tasks;
using SCMS.DomainEntities.Entities;
using SCMS.Contracts.DTOs.Responses;

namespace SCMS.Contracts.Interfaces.iRepositores
{
    public interface IUserRepository
    {
        // Hàm lấy tất cả user từ database (bất đồng bộ)
        Task<List<User>> GetAllUsersAsync();
        

        // Hàm thêm user mới vào database (bất đồng bộ)
        // User gợi từ class User trong SCMS.DomainEntities/Entities/User.cs
        Task<User> AddUserAsync(User user);
        // Hàm kiểm tra xem username đã tồn tại trong database chưa
        Task<bool> UsernameExistsAsync(string username);
        // Hàm kiểm tra xem email đã tồn tại trong database chưa
        Task<bool> EmailExistsAsync(string email);
        Task<User> UpdateUserAsync(User user);   // Cập nhật user
        
        // Hàm xóa user theo Id, trả về true nếu xóa thành công, false nếu user không tồn tại
        // bool : hàm bất đồng bộ trả về true/false để xác định kết quả xóa
        Task<bool> DeleteUserAsync(int userId);

    // Hàm vô hiệu hóa user theo Id, trả về true nếu thành công, false nếu user không tồn tại
       Task<bool> SetUserDisabledAsync(int userId, bool isDisabled);

        /// <summary>
        /// Tìm kiếm user theo keyword (username, email, fullname), status, isDisabled.
        /// Nếu tham số null sẽ bỏ qua điều kiện đó.
        /// </summary>
         Task<List<User>> SearchUsersAsync(string keyword, string status, bool? isDisabled);
         
         // Hàm lấy user theo trang (pagination), trả về danh sách user và tổng số user (để client biết có bao nhiêu trang)
   Task<(List<User> Users, int TotalCount)> GetUsersPagedAsync(int page, int pageSize);
   // Hàm tìm kiếm user theo keyword, status, isDisabled và phân trang, trả về danh sách user và tổng số user
   Task<(List<User> Users, int TotalCount)> SearchUsersPagedAsync(string keyword, string status, bool? isDisabled, int page, int pageSize);
    // Hàm lấy user theo username hoặc email (dùng cho đăng nhập)
    Task<User> GetUserByUsernameOrEmailAsync(string usernameOrEmail);

                    Task<User> GetByIdAsync(int userId); // Lấy user theo Id
    Task<User?> GetUserByEmailAsync(string email); // Nếu chưa có


// Hàm lấy tất cả avatar của user, trả về danh sách UserAvatarDto (chứa UserId và AvatarUrl)
    Task<IEnumerable<UserAvatarDto>> GetAllUserAvatarsAsync();
    // Hàm lấy avatar của user theo Id, trả về UserAvatarDto (chứa UserId và AvatarUrl), nếu user không có avatar thì trả về null
    Task<UserAvatarDto?> GetUserAvatarAsync(int userId);
    // Hàm cập nhật avatar của user, nếu user chưa có avatar thì tạo mới, nếu đã có avatar thì cập nhật lại, trả về UserAvatarDto (chứa UserId và AvatarUrl)
    Task<bool> DeleteUserAvatarAsync(int userId);
}
    
    
    }
