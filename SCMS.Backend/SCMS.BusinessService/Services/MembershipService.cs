using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.DTOs.Responses;
using SCMS.DomainEntities.Entities;
using SCMS.Contracts.Interfaces.iRepositores;
using SCMS.Contracts.Interfaces.iService;

namespace SCMS.BusinessService.Services
{

    public class MembershipService : IMembershipService
    {
        private readonly IMembershipRepository _membershipRepository;
        private readonly IUserRepository _userRepository;
        private readonly IClubRepository _clubRepository;


        public MembershipService(IMembershipRepository membershipRepository, IUserRepository userRepository, IClubRepository clubRepository)
        {
            _membershipRepository = membershipRepository;
            _userRepository = userRepository;
            _clubRepository = clubRepository;
        }

        // Triển khai đúng interface IMembershipService
        public async Task<MembershipResponse> RegisterAsync(CreateMembershipRequest request)
        {
            // TODO: Lấy userId từ context hoặc request tùy vào logic của bạn
            throw new NotImplementedException("Cần truyền userId hoặc lấy từ context.");
        }

        public async Task<MembershipResponse> RegisterAsync(CreateMembershipRequest request, int userId)
        {
            var existing = await _membershipRepository.GetByUserAndClubAsync(userId, request.ClubId);
            if (existing != null)
            {
                throw new Exception("Bạn đã đăng ký CLB này!");
            }

            // Kiểm tra trạng thái CLB
            var club = await _clubRepository.GetClubByIdAsync(request.ClubId);
            if (club == null)
            {
                throw new Exception("Không tìm thấy câu lạc bộ!");
            }
            if (string.Equals(club.Status, "Pending", StringComparison.OrdinalIgnoreCase)
                || string.Equals(club.Status, "Deleted", StringComparison.OrdinalIgnoreCase)
                || string.Equals(club.Status, "Inactive", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Không thể đăng ký tham gia CLB chưa được duyệt hoặc đã bị khóa/xóa!");
            }

            var membership = new Membership
            {
                UserId = userId,
                ClubId = request.ClubId,
                RegisterReason = request.RegisterReason,
                Skills = request.Skills,
                Experience = request.Experience,
                DesiredRole = request.DesiredRole,
                Role = "Thành viên",
                Status = "Pending",
                JoinedAt = null
            };

            var result = await _membershipRepository.AddAsync(membership);
            var user = await _userRepository.GetByIdAsync(userId);

            return MapToMembershipResponse(result, user);
        }

        public async Task<PagedResult<MembershipResponse>> GetMyApplicationsAsync(
            int userId,
            string? status,
            int page,
            int pageSize)
        {
            var (items, totalCount) = await _membershipRepository.GetMyApplicationsAsync(userId, status, page, pageSize);
            var user = await _userRepository.GetByIdAsync(userId);

            var responses = new List<MembershipResponse>();
            foreach (var item in items)
            {
                responses.Add(MapToMembershipResponse(item, user));
            }

            return new PagedResult<MembershipResponse>
            {
                Items = responses,
                TotalCount = totalCount,
                Page = page < 1 ? 1 : page,
                PageSize = pageSize < 1 ? 10 : pageSize
            };
        }

        public async Task<MembershipResponse?> GetMyApplicationDetailAsync(int membershipId, int userId)
        {
            var membership = await _membershipRepository.GetMyApplicationDetailAsync(membershipId, userId);
            if (membership == null)
            {
                throw new KeyNotFoundException("Không tìm thấy đơn đăng ký hoặc bạn không có quyền xem đơn này.");
            }

            var user = await _userRepository.GetByIdAsync(userId);
            return MapToMembershipResponse(membership, user);
        }

        public async Task<MembershipResponse> CancelMyApplicationAsync(int membershipId, int userId)
        {
            var membership = await _membershipRepository.GetMyApplicationDetailAsync(membershipId, userId);
            if (membership == null)
            {
                throw new KeyNotFoundException("Không tìm thấy đơn đăng ký hoặc bạn không có quyền hủy đơn này.");
            }

            if (!string.Equals(membership.Status, "Pending", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Chỉ có thể hủy đơn khi trạng thái đang là Pending.");
            }

            membership.Status = "Cancelled";
            var updated = await _membershipRepository.UpdateAsync(membership);
            var user = await _userRepository.GetByIdAsync(userId);

            return MapToMembershipResponse(updated, user);
        }

        public async Task<PagedResult<MembershipResponse>> GetMyJoinedClubsAsync(
            int userId,
            string? keyword,
            int page,
            int pageSize)
        {
            var (items, totalCount) = await _membershipRepository.GetMyJoinedClubsAsync(userId, keyword, page, pageSize);
            var user = await _userRepository.GetByIdAsync(userId);

            var responses = new List<MembershipResponse>();
            foreach (var item in items)
            {
                responses.Add(MapToMembershipResponse(item, user));
            }

            return new PagedResult<MembershipResponse>
            {
                Items = responses,
                TotalCount = totalCount,
                Page = page < 1 ? 1 : page,
                PageSize = pageSize < 1 ? 10 : pageSize
            };
        }

        public async Task<PagedResult<ClubMemberResponse>> GetMyClubMembersAsync(
            int userId,
            int clubId,
            string? keyword,
            int page,
            int pageSize)
        {
            var myMembership = await _membershipRepository.GetApprovedMembershipAsync(userId, clubId);
            if (myMembership == null)
            {
                throw new UnauthorizedAccessException("Bạn chưa tham gia CLB này hoặc không có quyền xem danh sách thành viên.");
            }

            var (items, totalCount) = await _membershipRepository.GetApprovedMembersByClubAsync(clubId, keyword, page, pageSize);

            var responses = new List<ClubMemberResponse>();
            foreach (var item in items)
            {
                responses.Add(new ClubMemberResponse
                {
                    MembershipId = item.MembershipId,
                    UserId = item.UserId,
                    FullName = item.User?.FullName,
                    Username = item.User?.Username,
                    Role = item.Role,
                    Status = item.Status,
                    JoinedAt = item.JoinedAt
                });
            }

            return new PagedResult<ClubMemberResponse>
            {
                Items = responses,
                TotalCount = totalCount,
                Page = page < 1 ? 1 : page,
                PageSize = pageSize < 1 ? 10 : pageSize
            };
        }

        public async Task<MembershipResponse> LeaveClubAsync(int clubId, int userId)
        {
            var membership = await _membershipRepository.GetApprovedMembershipAsync(userId, clubId);
            if (membership == null)
            {
                throw new KeyNotFoundException("Bạn chưa tham gia CLB này hoặc membership không ở trạng thái Approved.");
            }

            membership.Status = "Left";
            membership.LeftAt = DateTime.UtcNow;

            var updated = await _membershipRepository.UpdateAsync(membership);
            await _membershipRepository.SyncClubMemberCountAsync(clubId);
            var user = await _userRepository.GetByIdAsync(userId);

            return MapToMembershipResponse(updated, user);
        }

        private static MembershipResponse MapToMembershipResponse(Membership membership, User? user)
        {
            return new MembershipResponse
            {
                MembershipId = membership.MembershipId,
                UserId = membership.UserId,
                ClubId = membership.ClubId,
                ClubName = membership.Club?.ClubName,
                Role = membership.Role ?? "Thành viên",
                Status = membership.Status,
                JoinedAt = membership.JoinedAt,
                LeftAt = membership.LeftAt,
                RegisterReason = membership.RegisterReason,
                Skills = membership.Skills,
                Experience = membership.Experience,
                DesiredRole = membership.DesiredRole ?? "Thành viên",
                FullName = user?.FullName,
                Email = user?.Email,
                Phone = user?.Phone
            };
        }
    }
}