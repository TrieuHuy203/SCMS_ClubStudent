using System.Collections.Generic;
using System.Threading.Tasks;
using SCMS.DomainEntities.Entities;
using SCMS.Contracts.DTOs.Requests;

namespace SCMS.Contracts.Interfaces.iRepositores
{
    public interface IAdminMembershipRepository
    {
     // Tìm kiếm membership với filter và phân trang
        Task<List<Membership>> SearchAsync(AdminMembershipListRequestDto filter);
        // Đếm tổng số membership
        Task<int> CountAsync(AdminMembershipListRequestDto filter);

        // Lấy membership theo Id
        Task<Membership?> GetByIdAsync(int membershipId);

        // Cập nhật membership (duyệt, từ chối, cập nhật thông tin)
        Task UpdateAsync(Membership membership);

        // Đồng bộ MemberCount của club theo số membership đang Approved
        Task SyncClubMemberCountAsync(int clubId);
    }
}