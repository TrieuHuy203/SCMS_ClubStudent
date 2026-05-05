namespace SCMS.Contracts.Interfaces.iService;
using System.Collections.Generic;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.DTOs.Responses;

	public interface IAuthLogService
	{
		 Task<IEnumerable<AuthLogDto>> GetAuthLogsAsync(AuthLogSearchRequest request);
		Task LogAuthAsync(int userId, string actionType, string status, string? ipAddress = null, string? deviceInfo = null);
	}

