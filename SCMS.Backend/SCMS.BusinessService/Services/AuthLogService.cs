using SCMS.Contracts.Interfaces.iRepositores;
using SCMS.Contracts.Interfaces.iService;
using SCMS.DomainEntities.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.DTOs.Responses;


namespace SCMS.BusinessService.Services
{
	public class AuthLogService : IAuthLogService
	{
		private readonly IAuthLogRepository _authLogRepository;

		public AuthLogService(IAuthLogRepository authLogRepository)
		{
			_authLogRepository = authLogRepository;
		}

		public async Task LogAuthAsync(int userId, string actionType, string status, string? ipAddress = null, string? deviceInfo = null)
		{
			// Chuẩn hóa dữ liệu đầu vào để tránh lưu dữ liệu rác vào bảng log.
			var log = new AuthLog
			{
				UserId = userId,
				ActionType = (actionType ?? string.Empty).Trim().ToUpperInvariant(),
				Status = string.IsNullOrWhiteSpace(status) ? "SUCCESS" : status.Trim().ToUpperInvariant(),
				Ipaddress = string.IsNullOrWhiteSpace(ipAddress) ? null : ipAddress.Trim(),
				DeviceInfo = string.IsNullOrWhiteSpace(deviceInfo) ? null : deviceInfo.Trim(),
				ActionTime = DateTime.Now
			};

			await _authLogRepository.AddAsync(log);
		}
	
	  public async Task<IEnumerable<AuthLogDto>> GetAuthLogsAsync(AuthLogSearchRequest request)
    {
        return await _authLogRepository.GetAuthLogsAsync(request);
    }
	
	}
}
