namespace SCMS.Contracts.DTOs.Requests
{
    // Payload cho endpoint admin từ chối yêu cầu tạo CLB.
    public class RejectClubRequest
    {
        public string RejectReason { get; set; } = null!;
    }
}
