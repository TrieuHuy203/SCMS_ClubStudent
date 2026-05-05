using SCMS.DomainEntities.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.DTOs.Responses;

namespace SCMS.Contracts.Interfaces.iRepositores
{
	public interface IAuditLogRepository
	{
		Task AddAsync(AuditLog auditLog);
		 Task<IEnumerable<AuditLogDto>> GetAuditLogsAsync(AuditLogSearchRequest request);
	}
}
