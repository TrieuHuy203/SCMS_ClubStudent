using System;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.DTOs.Responses;
using SCMS.Contracts.Interfaces.iService;
using SCMS.Contracts.Interfaces.iRepositores;
using SCMS.DomainEntities.Entities;

namespace SCMS.BusinessService.Services
{
    public class AdminMembershipService : IAdminMembershipService
    {
        private readonly IAdminMembershipRepository _repo;
        private readonly IAuditLogService _auditLogService;

        public AdminMembershipService(IAdminMembershipRepository repo, IAuditLogService auditLogService)
        {
            _repo = repo;
            _auditLogService = auditLogService;
        }

        public async Task<AdminMembershipListResponseDto> SearchAsync(AdminMembershipListRequestDto filter)
        {
            var items = await _repo.SearchAsync(filter);
            var total = await _repo.CountAsync(filter);

            // Mapping sang DTO
            var result = new AdminMembershipListResponseDto
            {
                Items = items.Select(m => new AdminMembershipDetailResponseDto
                {
                    MembershipId = m.MembershipId,
                    UserId = m.UserId,
                    UserName = m.User.Username,
                    FullName = m.User.FullName,
                    ClubId = m.ClubId,
                    ClubName = m.Club.ClubName,
                    Role = m.Role,
                    DesiredRole = m.DesiredRole,
                    Status = m.Status,
                    RegisterReason = m.RegisterReason,
                    Skills = m.Skills,
                    Experience = m.Experience,
                    JoinedAt = m.JoinedAt,
                    LeftAt = m.LeftAt,
                    // ApproveOrRejectNote = ... (nếu có)
                }).ToList(),
                TotalCount = total
            };
            return result;
        }

        public async Task<AdminMembershipDetailResponseDto?> GetDetailAsync(int membershipId)
        {
            var m = await _repo.GetByIdAsync(membershipId);
            if (m == null) return null;
            return new AdminMembershipDetailResponseDto
            {
                MembershipId = m.MembershipId,
                UserId = m.UserId,
                UserName = m.User.Username,
                FullName = m.User.FullName,
                ClubId = m.ClubId,
                ClubName = m.Club.ClubName,
                Role = m.Role,
                DesiredRole = m.DesiredRole,
                Status = m.Status,
                RegisterReason = m.RegisterReason,
                Skills = m.Skills,
                Experience = m.Experience,
                JoinedAt = m.JoinedAt,
                LeftAt = m.LeftAt,
                // ApproveOrRejectNote = ... (nếu có)
            };
        }

        public async Task ApproveAsync(int membershipId, int adminId, string? note = null)
        {
            var m = await _repo.GetByIdAsync(membershipId);
            if (m == null) throw new Exception("Membership not found");
            m.Status = "Approved";
            // m.ApproveOrRejectNote = note; // nếu có field này
            await _repo.UpdateAsync(m);
            await _repo.SyncClubMemberCountAsync(m.ClubId);

            // Ghi lại ai đã duyệt membership nào.
            await SafeWriteAuditLogAsync(adminId, "Membership", membershipId, "APPROVE", null, note);
        }

        public async Task RejectAsync(int membershipId, int adminId, string? reason = null)
        {
            var m = await _repo.GetByIdAsync(membershipId);
            if (m == null) throw new Exception("Membership not found");
            m.Status = "Rejected";
            // m.ApproveOrRejectNote = reason; // nếu có field này
            await _repo.UpdateAsync(m);
            await _repo.SyncClubMemberCountAsync(m.ClubId);

            await SafeWriteAuditLogAsync(adminId, "Membership", membershipId, "REJECT", null, reason);
        }

        public async Task KickAsync(int membershipId, int adminId)
        {
            var m = await _repo.GetByIdAsync(membershipId);
            if (m == null) throw new Exception("Membership not found");

            if (!string.Equals(m.Status, "Approved", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Chỉ có thể kick thành viên khi membership đang ở trạng thái Approved.");
            }

            m.Status = "Left";
            m.LeftAt = DateTime.UtcNow;

            await _repo.UpdateAsync(m);
            await _repo.SyncClubMemberCountAsync(m.ClubId);

            await SafeWriteAuditLogAsync(adminId, "Membership", membershipId, "KICK");
        }

        // Best effort: nếu ghi audit lỗi thì không làm fail nghiệp vụ chính.
        private async Task SafeWriteAuditLogAsync(int actorUserId, string tableName, int recordId, string actionType, string? oldValue = null, string? newValue = null)
        {
            try
            {
                await _auditLogService.LogAuditAsync(actorUserId, tableName, recordId, actionType, oldValue, newValue);
            }
            catch
            {
                // Intentionally ignore audit log errors.
            }
        }
    }
}