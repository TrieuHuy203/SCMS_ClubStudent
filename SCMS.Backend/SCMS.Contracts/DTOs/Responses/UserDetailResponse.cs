namespace SCMS.Contracts.DTOs.Responses
{
    public class UserDetailResponse
    {
        public int Id { get; set; }              // map từ UserId
        public string ?Username { get; set; }
        public string ?FullName { get; set; }
        public string ?Email { get; set; }
        public string ?Phone { get; set; }
        public string ?Status { get; set; }
        public bool IsDisabled { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> ?Roles { get; set; }  // nếu muốn trả về roles
    }
}