using System.Collections.Generic;

namespace SCMS.Contracts.DTOs.Responses
{
	public class AdminMembershipListResponseDto
	{
		public List<AdminMembershipDetailResponseDto> Items { get; set; } = new();
		public int TotalCount { get; set; }
	}
}
