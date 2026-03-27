thứ tự code BE
DTOs (SCMS.Contracts/DTOs/UserDto.cs, ...):

Định nghĩa các class cho dữ liệu vào/ra (UserCreateDto, UserUpdateDto, UserDetailDto, UserSearchDto, ...).
Repository Interface (SCMS.Contracts/Interfaces/IUserRepository.cs):

Định nghĩa các method CRUD, search, soft delete, check trùng lặp, v.v.
Repository Implementation (SCMS.DAL/Repositories/UserRepository.cs):

Cài đặt các method của IUserRepository, thao tác với DbContext.
Service Interface (SCMS.Contracts/Interfaces/IUserService.cs):

Định nghĩa các method nghiệp vụ (gọi repository, xử lý logic).
Service Implementation (SCMS.BusinessService/Services/UserService.cs):

Cài đặt các method của IUserService, xử lý logic nghiệp vụ, kiểm tra trùng lặp, validate, ...
Controller (SCMS.WebAPI/Controllers/UserController.cs):

Định nghĩa các API endpoint, nhận request, trả response, inject service.
Đăng ký DI (SCMS.WebAPI/Program.cs hoặc Startup.cs):

Đăng ký IUserRepository, IUserService vào DI container.
(Tùy chọn) Unit Test cho Service/Repository (nếu có):

Đảm bảo code đúng, dễ bảo trì.



/////////////////////////////////////////////////////////////

quản lý user
Thêm user (Create)
Sửa user (Update)
Xoá mềm user (Soft Delete)
Vô hiệu hoá / Kích hoạt lại tài khoản (Disable/Enable)
Tìm kiếm / Lọc user (Search/Filter)
Xem chi tiết user (Get by Id)
Đổi mật khẩu / Reset mật khẩu (Change/Reset Password)
Kiểm tra trùng lặp email/username khi thêm/sửa