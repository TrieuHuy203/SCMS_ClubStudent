using SCMS.Contracts.Interfaces.iRepositores;
using SCMS.Contracts.Interfaces.iService;
using SCMS.DomainEntities.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using SCMS.Contracts.DTOs.Requests;
using SCMS.Contracts.DTOs.Responses;


namespace SCMS.BusinessService.Services
{
	public class AuditLogService : IAuditLogService
	{
		private readonly IAuditLogRepository _auditLogRepository;

		public AuditLogService(IAuditLogRepository auditLogRepository)
		{
			_auditLogRepository = auditLogRepository;
		}

		public async Task LogAuditAsync(
			int userId,
			string tableName,
			int recordId,
			string actionType,
			string? oldValue = null,
			string? newValue = null)
		{
			// Chuẩn hóa dữ liệu để đồng nhất format log và giảm lỗi do vượt quá độ dài cột.
			var auditLog = new AuditLog
			{
				UserId = userId,
				TableName = NormalizeText(tableName, 100),
				RecordId = recordId,
				ActionType = NormalizeText(actionType, 20).ToUpperInvariant(),
				OldValue = string.IsNullOrWhiteSpace(oldValue) ? null : oldValue,
				NewValue = string.IsNullOrWhiteSpace(newValue) ? null : newValue,
				ActionTime = DateTime.Now
			};

			await _auditLogRepository.AddAsync(auditLog);
		}

		// Cắt độ dài theo schema DB để tránh lỗi SQL khi lưu log.
		private static string NormalizeText(string? value, int maxLength)
		{
			var normalized = (value ?? string.Empty).Trim();
			if (normalized.Length <= maxLength)
				return normalized;

			return normalized.Substring(0, maxLength);
		}
	 public async Task<IEnumerable<AuditLogDto>> GetAuditLogsAsync(AuditLogSearchRequest request)
    {
        return await _auditLogRepository.GetAuditLogsAsync(request);
    }}
}
