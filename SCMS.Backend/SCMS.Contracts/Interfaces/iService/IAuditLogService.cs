namespace SCMS.Contracts.Interfaces.iService;
using System.Collections.Generic;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.DTOs.Responses;

	public interface IAuditLogService
	{
		  Task<IEnumerable<AuditLogDto>> GetAuditLogsAsync(AuditLogSearchRequest request);

		Task LogAuditAsync(
			int userId,
			string tableName,
			int recordId,
			string actionType,
			string? oldValue = null,
			string? newValue = null);
	}

