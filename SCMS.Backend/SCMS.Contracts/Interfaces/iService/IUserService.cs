// Interface định nghĩa các hàm xử lý nghiệp vụ liên quan đến User ở tầng Service
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.DTOs.Responses;
using SCMS.DomainEntities.Entities;

namespace SCMS.Contracts.Interfaces.iService
{
    public interface IUserService
    {

        // Hàm lấy tất cả user, trả về danh sách DTO response (UserResponse)
        Task<List<User>> GetAllUsersAsync();
        // Hàm tạo user mới, nhận vào DTO request, trả về DTO response
        // UserCreateResponse sẽ gọi từ class UserCreateResponse trong SCMS.Contracts/DTOs/Responses/UserCreateResponse.cs
        Task<UserCreateResponse> CreateUserAsync(UserCreateRequest request);

        // Hàm cập nhật thông tin user, nhận vào userId và DTO request, trả về DTO response
         Task<UserUpdateResponse> UpdateUserAsync(int userId, UserUpdateRequest request);

         // Hàm xóa user theo Id, trả về true nếu xóa thành công, false nếu user không tồn tại
        Task<bool> DeleteUserAsync(int userId, int currentUserId);
 
        // Hàm vô hiệu hóa user theo Id, trả về true nếu thành công, false nếu user không tồn tại hoặc cố gắng vô hiệu hóa chính mình
        Task<bool> SetUserDisabledAsync(int userId, bool isDisabled, int currentUserId);
        // Hàm lấy chi tiết thông tin user theo Id, trả về DTO response UserDetailResponse
        Task<UserDetailResponse> GetUserDetailAsync(int userId);
 /// <summary>
        /// Tìm kiếm user theo keyword, status, isDisabled (nâng cao).
        /// </summary>
        Task<List<UserDetailResponse>> SearchUsersAsync(string keyword, string status, bool? isDisabled);
   // Hàm lấy user theo trang (pagination), trả về danh sách DTO response và tổng số user
   Task<PagedResult<UserDetailResponse>> GetUsersPagedAsync(int page, int pageSize);
   // Hàm tìm kiếm user theo keyword, status, isDisabled và phân trang, trả về danh sách DTO response và tổng số user
   Task<PagedResult<UserDetailResponse>> SearchUsersPagedAsync(UserSearchRequest request);
   // Hàm đăng nhập, nhận vào DTO request UserLoginRequest, trả về DTO response UserLoginResponse
   Task<UserLoginResponse> LoginAsync(UserLoginRequest request);

// Hàm xác nhận email, nhận vào DTO request ConfirmEmailRequest, trả về true nếu xác nhận thành công, false nếu thất bại
    Task<bool> ConfirmEmailAsync(string email, string token);
    // Hàm lấy user theo username hoặc email (dùng cho đăng nhập)
        Task<User?> GetUserByEmailAsync(string email);
   
    }
 }
