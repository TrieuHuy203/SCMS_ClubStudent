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

        // Inject repository qua constructor
        public ClubService(IClubRepository clubRepository)
        {
            _clubRepository = clubRepository;
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
                CreatedAt = c.CreatedAt
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
                CreatedAt = club.CreatedAt
            };
        }

        // Tạo mới club
       // Tạo mới club (có validate, check trùng tên, set mặc định)
public async Task<ClubResponse> CreateClubAsync(ClubCreateRequest request)
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
        Status = "Active", // luôn mặc định là Active
        CreatedAt = DateTime.Now // luôn lấy thời điểm hiện tại
    };

    var created = await _clubRepository.CreateClubAsync(club);

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
        CreatedAt = created.CreatedAt
    };
}

        // Cập nhật club
     // Cập nhật club (có validate, check trùng tên, không cho sửa CreatedAt, MemberCount, kiểm tra status)
        public async Task UpdateClubAsync(ClubUpdateRequest request)
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

            await _clubRepository.UpdateClubAsync(club);
        }

        // Xóa club
        // Xóa mềm club (chỉ cập nhật Status = "Deleted")
        public async Task DeleteClubAsync(int clubId)
        {
            // 1. Kiểm tra tồn tại
            var club = await _clubRepository.GetClubByIdAsync(clubId);
            if (club == null)
                throw new KeyNotFoundException("Không tìm thấy câu lạc bộ!");

            // 2. Nếu đã bị xóa mềm thì không xóa nữa
            if (club.Status == "Deleted")
                throw new InvalidOperationException("Câu lạc bộ đã bị xóa trước đó!");

            // 3. Cập nhật trạng thái
            club.Status = "Deleted";
            await _clubRepository.UpdateClubAsync(club);
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
                CreatedAt = c.CreatedAt
            }).ToList();

            return new PagedResult<ClubResponse>
            {
                Items = items,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}