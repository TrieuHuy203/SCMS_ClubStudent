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
	public class AuditLogRepository : IAuditLogRepository
	{
		private readonly SCMSDbContext _context;

		public AuditLogRepository(SCMSDbContext context)
		{
			_context = context;
		}

		public async Task AddAsync(AuditLog auditLog)
		{
			await _context.AuditLogs.AddAsync(auditLog);
			await _context.SaveChangesAsync();
		}
	
	
	 public async Task<IEnumerable<AuditLogDto>> GetAuditLogsAsync(AuditLogSearchRequest request)
    {
        var query = _context.AuditLogs.AsQueryable();

        if (request.UserId.HasValue)
            query = query.Where(x => x.UserId == request.UserId.Value);
        if (!string.IsNullOrEmpty(request.TableName))
            query = query.Where(x => x.TableName == request.TableName);
        if (request.RecordId.HasValue)
            query = query.Where(x => x.RecordId == request.RecordId.Value);
        if (!string.IsNullOrEmpty(request.ActionType))
            query = query.Where(x => x.ActionType == request.ActionType);
        if (request.FromDate.HasValue)
            query = query.Where(x => x.ActionTime >= request.FromDate.Value);
        if (request.ToDate.HasValue)
            query = query.Where(x => x.ActionTime <= request.ToDate.Value);

        var result = await query
            .OrderByDescending(x => x.ActionTime)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new AuditLogDto
            {
                AuditLogId = x.AuditLogId,
                UserId = x.UserId,
                TableName = x.TableName,
                RecordId = x.RecordId,
                ActionType = x.ActionType,
                OldValue = x.OldValue,
                NewValue = x.NewValue,
                ActionTime = x.ActionTime
                // UserName = x.User.FullName // nếu muốn join với bảng User
            })
            .ToListAsync();

        return result;
    }
	}
}
