namespace SCMS.Contracts.DTOs.Requests
{
    public class AdminResetPasswordRequest
    {
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}