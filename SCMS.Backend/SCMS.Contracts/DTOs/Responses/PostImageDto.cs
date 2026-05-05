namespace SCMS.Contracts.DTOs.Responses
{
    public class PostImageDto
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string ImageUrl { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        // Thêm các trường khác nếu cần
    }
}