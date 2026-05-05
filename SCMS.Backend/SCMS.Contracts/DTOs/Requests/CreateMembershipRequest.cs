namespace SCMS.Contracts.DTOs.Requests
{
    public class CreateMembershipRequest
    {
        public int ClubId { get; set; } // CLB muốn tham gia

        // Các trường nhập khi đăng ký
        public string? RegisterReason { get; set; }    // Lý do muốn tham gia CLB
        public string? Skills { get; set; }            // Kỹ năng/sở thích liên quan
        public string? Experience { get; set; }        // Kinh nghiệm tham gia CLB/hoạt động ngoại khóa
        /// <summary>
        /// Vai trò mong muốn: "Thành viên", "Chủ nhiệm", "Phó chủ nhiệm"
        /// </summary>
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.RegularExpression(@"^(Thành viên|Chủ nhiệm|Phó chủ nhiệm)$", ErrorMessage = "Vai trò mong muốn chỉ được chọn: Thành viên, Chủ nhiệm, Phó chủ nhiệm")]
        public required string DesiredRole { get; set; } // Vai trò mong muốn
    }
}