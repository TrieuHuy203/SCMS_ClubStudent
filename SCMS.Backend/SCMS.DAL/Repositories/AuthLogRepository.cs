using SCMS.Contracts.Interfaces.iRepositores;
using SCMS.DomainEntities.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.DTOs.Responses;

namespace SCMS.DAL.Repositories
{
	public class AuthLogRepository : IAuthLogRepository
	{
		private readonly SCMSDbContext _context;

		public AuthLogRepository(SCMSDbContext context)
		{
			_context = context;
		}

		public async Task AddAsync(AuthLog authLog)
		{
			await _context.AuthLogs.AddAsync(authLog);
			await _context.SaveChangesAsync();
		}

 public async Task<IEnumerable<AuthLogDto>> GetAuthLogsAsync(AuthLogSearchRequest request)
    {
        var query = _context.AuthLogs.AsQueryable();

        if (request.UserId.HasValue)
            query = query.Where(x => x.UserId == request.UserId.Value);
        if (!string.IsNullOrEmpty(request.ActionType))
            query = query.Where(x => x.ActionType == request.ActionType);
        if (!string.IsNullOrEmpty(request.Status))
            query = query.Where(x => x.Status == request.Status);
        if (request.FromDate.HasValue)
            query = query.Where(x => x.ActionTime >= request.FromDate.Value);
        if (request.ToDate.HasValue)
            query = query.Where(x => x.ActionTime <= request.ToDate.Value);

        var result = await query
            .OrderByDescending(x => x.ActionTime)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new AuthLogDto
            {
                AuthLogId = x.AuthLogId,
                UserId = x.UserId,
                ActionType = x.ActionType,
                DeviceInfo = x.DeviceInfo,
                IPAddress = x.Ipaddress,
                ActionTime = x.ActionTime,
                Status = x.Status
                // UserName = x.User.UserName // nếu muốn join với bảng User
            })
            .ToListAsync();

        return result;
    }

	}
}
