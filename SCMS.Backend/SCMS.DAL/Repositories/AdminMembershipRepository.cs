using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SCMS.DomainEntities.Entities;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.Interfaces.iRepositores;

namespace SCMS.DAL.Repositories
{
    public class AdminMembershipRepository : IAdminMembershipRepository
    {
        private readonly SCMSDbContext _context;

        public AdminMembershipRepository(SCMSDbContext context)
        {
            _context = context;
        }

        public async Task<List<Membership>> SearchAsync(AdminMembershipListRequestDto filter)
        {
            var query = _context.Memberships.AsQueryable();

            if (filter.ClubId.HasValue)
                query = query.Where(x => x.ClubId == filter.ClubId.Value);

                if (!string.IsNullOrEmpty(filter.UserName))
                    query = query.Where(x => x.User.FullName.Contains(filter.UserName) || x.User.Username.Contains(filter.UserName));

            if (!string.IsNullOrEmpty(filter.Status))
                query = query.Where(x => x.Status == filter.Status);

            return await query
                .Include(x => x.User)
                .Include(x => x.Club)
                .OrderByDescending(x => x.MembershipId)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();
        }

        public async Task<int> CountAsync(AdminMembershipListRequestDto filter)
        {
            var query = _context.Memberships.AsQueryable();

            if (filter.ClubId.HasValue)
                query = query.Where(x => x.ClubId == filter.ClubId.Value);

                if (!string.IsNullOrEmpty(filter.UserName))
                    query = query.Where(x => x.User.FullName.Contains(filter.UserName) || x.User.Username.Contains(filter.UserName));

            if (!string.IsNullOrEmpty(filter.Status))
                query = query.Where(x => x.Status == filter.Status);

            return await query.CountAsync();
        }

        public async Task<Membership?> GetByIdAsync(int membershipId)
        {
            return await _context.Memberships
                .Include(x => x.User)
                .Include(x => x.Club)
                .FirstOrDefaultAsync(x => x.MembershipId == membershipId);
        }

        public async Task UpdateAsync(Membership membership)
        {
            _context.Memberships.Update(membership);
            await _context.SaveChangesAsync();
        }

        public async Task SyncClubMemberCountAsync(int clubId)
        {
            var approvedCount = await _context.Memberships
                .CountAsync(x => x.ClubId == clubId
                    && x.Status != null
                    && x.Status.ToLower() == "approved");

            var club = await _context.Clubs.FirstOrDefaultAsync(x => x.ClubId == clubId);
            if (club == null)
            {
                return;
            }

            club.MemberCount = approvedCount;
            await _context.SaveChangesAsync();
        }
    }
}