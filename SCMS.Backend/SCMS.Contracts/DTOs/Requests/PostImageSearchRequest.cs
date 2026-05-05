namespace SCMS.Contracts.DTOs.Requests

{
    public class PostImageSearchRequest
    {
        public int? PostId { get; set; }
        public int? UserId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int? ClubId { get; set; }
        // Thêm các trường lọc khác nếu cần
    }
}