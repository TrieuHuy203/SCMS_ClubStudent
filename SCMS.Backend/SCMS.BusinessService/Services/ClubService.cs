using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.DTOs.Responses;
using SCMS.Contracts.Interfaces.iService;
using SCMS.Contracts.Interfaces.iRepositores;
using SCMS.DomainEntities.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCMS.BusinessService.Services
{
    // Triển khai các phương thức nghiệp vụ cho Club
    public class ClubService : IClubService
    {
        private readonly IClubRepository _clubRepository;
        private readonly IAuditLogService _auditLogService;

        // Inject repository qua constructor
        public ClubService(IClubRepository clubRepository, IAuditLogService auditLogService)
        {
            _clubRepository = clubRepository;
            _auditLogService = auditLogService;
        }

        // Lấy tất cả các club
        public async Task<IEnumerable<ClubResponse>> GetAllClubsAsync()
        {
            var clubs = await _clubRepository.GetAllClubsAsync();
            // Map entity sang DTO trả về
            return clubs.Select(c => new ClubResponse
            {
                ClubId = c.ClubId,
                ClubName = c.ClubName,
                Description = c.Description,
                Field = c.Field,
                School = c.School,
                MemberCount = c.MemberCount,
                Status = c.Status,
                RejectReason = c.RejectReason,
                CreatedAt = c.CreatedAt,
                CreatedByUserId = c.CreatedByUserId
            });
        }

        // Lấy club theo Id
        public async Task<ClubResponse> GetClubByIdAsync(int clubId)
        {
            var club = await _clubRepository.GetClubByIdAsync(clubId);
            if (club == null) return null;
            return new ClubResponse
            {
                ClubId = club.ClubId,
                ClubName = club.ClubName,
                Description = club.Description,
                Field = club.Field,
                School = club.School,
                MemberCount = club.MemberCount,
                Status = club.Status,
                RejectReason = club.RejectReason,
                CreatedAt = club.CreatedAt,
                CreatedByUserId = club.CreatedByUserId
            };
        }

        // Tạo mới club
       // Tạo mới club (có validate, check trùng tên, set mặc định)
public async Task<ClubResponse> CreateClubAsync(ClubCreateRequest request, int actorUserId)
{
    // 1. Validate dữ liệu đầu vào
    if (string.IsNullOrWhiteSpace(request.ClubName))
        throw new ArgumentException("ClubName không được để trống!");

    // 2. Check trùng tên (không phân biệt hoa thường)
    var allClubs = await _clubRepository.GetAllClubsAsync();
    if (allClubs.Any(c => c.ClubName.Trim().ToLower() == request.ClubName.Trim().ToLower()))
        throw new InvalidOperationException("Tên câu lạc bộ đã tồn tại!");

    // 3. Tạo entity và set mặc định
    var club = new Club
    {
        ClubName = request.ClubName.Trim(),
        Description = request.Description,
        Field = request.Field,
        School = request.School,
        MemberCount = 0, // luôn mặc định 0 khi tạo mới
        IsDisabled = false, // mặc định chưa bị vô hiệu hóa
        Status = "Active", // luôn mặc định là Active
        RejectReason = null,
        CreatedAt = DateTime.Now, // luôn lấy thời điểm hiện tại
        CreatedByUserId = actorUserId // Gán userId người tạo
    };

    var created = await _clubRepository.CreateClubAsync(club);

    await SafeWriteAuditLogAsync(actorUserId, "Club", created.ClubId, "CREATE");

    // 4. Map sang DTO trả về
    return new ClubResponse
    {
        ClubId = created.ClubId,
        ClubName = created.ClubName,
        Description = created.Description,
        Field = created.Field,
        School = created.School,
        MemberCount = created.MemberCount,
        Status = created.Status,
        RejectReason = created.RejectReason,
        CreatedAt = created.CreatedAt,
        CreatedByUserId = created.CreatedByUserId
    };
}

        // User gửi yêu cầu tạo club, trạng thái mặc định Pending
        public async Task<ClubResponse> SubmitClubRegistrationAsync(ClubCreateRequest request, int userId)
        {
            if (string.IsNullOrWhiteSpace(request.ClubName))
                throw new ArgumentException("ClubName không được để trống!");

            var allClubs = await _clubRepository.GetAllClubsAsync();
            if (allClubs.Any(c => c.ClubName.Trim().ToLower() == request.ClubName.Trim().ToLower()))
                throw new InvalidOperationException("Tên câu lạc bộ đã tồn tại!");

            var club = new Club
            {
                ClubName = request.ClubName.Trim(),
                Description = request.Description,
                Field = request.Field,
                School = request.School,
                MemberCount = 0,
                IsDisabled = true,
                Status = "Pending",
                // Đơn mới luôn xóa lý do từ chối cũ (nếu có).
                RejectReason = null,
                CreatedAt = DateTime.Now,
                CreatedByUserId = userId // Gán userId người đăng ký tạo CLB
            };

            var created = await _clubRepository.CreateClubAsync(club);

            // User gửi yêu cầu mở CLB mới (trạng thái Pending).
            await SafeWriteAuditLogAsync(userId, "Club", created.ClubId, "SUBMIT");

            return new ClubResponse
            {
                ClubId = created.ClubId,
                ClubName = created.ClubName,
                Description = created.Description,
                Field = created.Field,
                School = created.School,
                MemberCount = created.MemberCount,
                Status = created.Status,
                RejectReason = created.RejectReason,
                CreatedAt = created.CreatedAt
            };
        }

        // Cập nhật club
     // Cập nhật club (có validate, check trùng tên, không cho sửa CreatedAt, MemberCount, kiểm tra status)
        public async Task UpdateClubAsync(ClubUpdateRequest request, int actorUserId)
        {
            // 1. Kiểm tra tồn tại
            var club = await _clubRepository.GetClubByIdAsync(request.ClubId);
            if (club == null)
                throw new KeyNotFoundException("Không tìm thấy câu lạc bộ!");

            // 2. Không cho sửa nếu đã bị xóa mềm
            if (club.Status == "Deleted")
                throw new InvalidOperationException("Không thể cập nhật club đã bị xóa!");

            // 3. Validate ClubName
            if (string.IsNullOrWhiteSpace(request.ClubName))
                throw new ArgumentException("ClubName không được để trống!");

            // 4. Check trùng tên (ngoại trừ chính nó)
            var allClubs = await _clubRepository.GetAllClubsAsync();
            if (allClubs.Any(c => c.ClubId != club.ClubId && c.ClubName.Trim().ToLower() == request.ClubName.Trim().ToLower()))
                throw new InvalidOperationException("Tên câu lạc bộ đã tồn tại!");

            // 5. Cập nhật các trường cho phép
            club.ClubName = request.ClubName.Trim();
            club.Description = request.Description;
            club.Field = request.Field;
            club.School = request.School;
            // Không cho sửa MemberCount, CreatedAt
            club.Status = request.Status;
            // Nếu admin chuyển lại Pending/Active thì clear lý do từ chối để tránh dữ liệu cũ.
            if (string.Equals(request.Status, "Pending", StringComparison.OrdinalIgnoreCase)
                || string.Equals(request.Status, "Active", StringComparison.OrdinalIgnoreCase))
            {
                club.RejectReason = null;
            }

            await _clubRepository.UpdateClubAsync(club);

            await SafeWriteAuditLogAsync(actorUserId, "Club", club.ClubId, "UPDATE");
        }

        // Xóa club
        // Xóa mềm club (chỉ cập nhật Status = "Deleted")
        public async Task DeleteClubAsync(int clubId, int actorUserId)
        {
            // 1. Kiểm tra tồn tại
            var club = await _clubRepository.GetClubByIdAsync(clubId);
            if (club == null)
                throw new KeyNotFoundException("Không tìm thấy câu lạc bộ!");

            // 2. Nếu đã bị xóa mềm thì không xóa nữa
            if (club.Status == "Deleted")
                throw new InvalidOperationException("Câu lạc bộ đã bị xóa trước đó!");

            // 3. Xóa mềm ở repository (đồng bộ IsDisabled + Status)
            await _clubRepository.DeleteClubAsync(clubId);

            await SafeWriteAuditLogAsync(actorUserId, "Club", clubId, "DELETE");
        }

        // Bật/tắt hoạt động club
        public async Task<bool> SetClubDisabledAsync(int clubId, bool isDisabled, int actorUserId)
        {
            var club = await _clubRepository.GetClubByIdAsync(clubId);
            if (club == null)
                return false;

            // Không cho bật/tắt club đã xóa mềm
            if (club.Status == "Deleted")
                throw new InvalidOperationException("Không thể thay đổi trạng thái club đã xóa.");

            var updated = await _clubRepository.SetClubDisabledAsync(clubId, isDisabled);

            if (updated)
            {
                await SafeWriteAuditLogAsync(actorUserId, "Club", clubId, isDisabled ? "DISABLE" : "ENABLE");
            }

            return updated;
        }

// Lấy club theo trang (pagination), trả về danh sách club và tổng số club (để client biết có bao nhiêu trang)
        public async Task<PagedResult<ClubResponse>> GetClubsPagedAsync(int page, int pageSize)
{
    var (clubs, totalCount) = await _clubRepository.GetClubsPagedAsync(page, pageSize);
    var items = clubs.Select(c => new ClubResponse
    {
        ClubId = c.ClubId,
        ClubName = c.ClubName,
        Description = c.Description,
        Field = c.Field,
        School = c.School,
        MemberCount = c.MemberCount,
        Status = c.Status,
        RejectReason = c.RejectReason,
        CreatedAt = c.CreatedAt
    }).ToList();

    return new PagedResult<ClubResponse>
    {
        Items = items,
        TotalCount = totalCount,
        Page = page,
        PageSize = pageSize
    };
}


 // Tìm kiếm & phân trang club
        public async Task<PagedResult<ClubResponse>> SearchClubsPagedAsync(ClubSearchRequest request)
        {
            var (clubs, totalCount) = await _clubRepository.SearchClubsPagedAsync(request);

            var items = clubs.Select(c => new ClubResponse
            {
                ClubId = c.ClubId,
                ClubName = c.ClubName,
                Description = c.Description,
                Field = c.Field,
                School = c.School,
                MemberCount = c.MemberCount,
                Status = c.Status,
                RejectReason = c.RejectReason,
                CreatedAt = c.CreatedAt,
                CreatedByUserId = c.CreatedByUserId
            }).ToList();

            return new PagedResult<ClubResponse>
            {
                Items = items,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }

        // Admin xem danh sách club chờ duyệt
        public async Task<PagedResult<ClubResponse>> GetPendingClubsAsync(int page, int pageSize)
        {
            var request = new ClubSearchRequest
            {
                Status = "Pending",
                Page = page < 1 ? 1 : page,
                PageSize = pageSize < 1 ? 10 : pageSize
            };

            return await SearchClubsPagedAsync(request);
        }

        // Admin duyệt yêu cầu tạo club
        public async Task<ClubResponse> ApproveClubAsync(int clubId, int actorUserId)
        {
            var club = await _clubRepository.GetClubByIdAsync(clubId);
            if (club == null)
                throw new KeyNotFoundException("Không tìm thấy câu lạc bộ!");

            if (string.Equals(club.Status, "Deleted", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Không thể duyệt club đã xóa.");

            club.Status = "Active";
            club.IsDisabled = false;
            club.RejectReason = null;

            await _clubRepository.UpdateClubAsync(club);

            await SafeWriteAuditLogAsync(actorUserId, "Club", clubId, "APPROVE");

            return new ClubResponse
            {
                ClubId = club.ClubId,
                ClubName = club.ClubName,
                Description = club.Description,
                Field = club.Field,
                School = club.School,
                MemberCount = club.MemberCount,
                Status = club.Status,
                RejectReason = club.RejectReason,
                CreatedAt = club.CreatedAt,
                CreatedByUserId = club.CreatedByUserId
            };
        }

        // Admin từ chối yêu cầu tạo club
        public async Task<ClubResponse> RejectClubAsync(int clubId, int actorUserId, string rejectReason)
        {
            if (string.IsNullOrWhiteSpace(rejectReason))
                throw new ArgumentException("Lý do từ chối không được để trống.");

            var club = await _clubRepository.GetClubByIdAsync(clubId);
            if (club == null)
                throw new KeyNotFoundException("Không tìm thấy câu lạc bộ!");

            if (string.Equals(club.Status, "Deleted", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Không thể từ chối club đã xóa.");

            club.Status = "Rejected";
            club.IsDisabled = true;
            // Lưu lý do để user đọc và chỉnh sửa hồ sơ đăng ký.
            club.RejectReason = rejectReason.Trim();

            await _clubRepository.UpdateClubAsync(club);

            await SafeWriteAuditLogAsync(actorUserId, "Club", clubId, "REJECT", null, rejectReason.Trim());

            return new ClubResponse
            {
                ClubId = club.ClubId,
                ClubName = club.ClubName,
                Description = club.Description,
                Field = club.Field,
                School = club.School,
                MemberCount = club.MemberCount,
                Status = club.Status,
                RejectReason = club.RejectReason,
                CreatedAt = club.CreatedAt,
                CreatedByUserId = club.CreatedByUserId
            };
        }

        // Best effort: lỗi audit không làm fail luồng nghiệp vụ.
        private async Task SafeWriteAuditLogAsync(int actorUserId, string tableName, int recordId, string actionType, string? oldValue = null, string? newValue = null)
        {
            try
            {
                await _auditLogService.LogAuditAsync(actorUserId, tableName, recordId, actionType, oldValue, newValue);
            }
            catch
            {
                // Intentionally swallow audit errors.
            }
        
        
        }
        // Lấy thống kê số lượng thành viên, bài viết, sự kiện, bình luận của một club
  public async Task<ClubStatisticsDto> GetStatisticsAsync(int clubId)
{
    return await _clubRepository.GetStatisticsAsync(clubId);
}
  
    }
}