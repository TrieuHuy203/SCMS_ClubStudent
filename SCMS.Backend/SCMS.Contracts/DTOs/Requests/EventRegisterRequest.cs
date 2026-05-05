using System.ComponentModel.DataAnnotations;

namespace SCMS.Contracts.DTOs.Requests
{
	public class EventRegisterRequest
	{
		// Id sự kiện user muốn đăng ký tham gia
		[Required]
		public int EventId { get; set; }
	}
}
