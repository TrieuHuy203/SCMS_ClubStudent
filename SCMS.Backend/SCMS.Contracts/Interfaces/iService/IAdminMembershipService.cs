using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.DTOs.Responses;

namespace SCMS.Contracts.Interfaces.iService
{
    public interface IAdminMembershipService
    {
        Task<AdminMembershipListResponseDto> SearchAsync(AdminMembershipListRequestDto filter);
        Task<AdminMembershipDetailResponseDto?> GetDetailAsync(int membershipId);
        // Duyệt đơn đăng ký tham gia CLB
        Task ApproveAsync(int membershipId, int adminId, string? note = null);
        // Từ chối đơn đăng ký tham gia CLB
        Task RejectAsync(int membershipId, int adminId, string? reason = null);
        // Kick thành viên đã được duyệt khỏi CLB
        Task KickAsync(int membershipId, int adminId);
        
    }
}