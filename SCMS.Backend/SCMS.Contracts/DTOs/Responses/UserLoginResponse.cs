public class UserLoginResponse
{
    public int UserId { get; set; }
    public string ?Username { get; set; }
    public string ?Token { get; set; } // Nếu dùng JWT
    public string ?Message { get; set; }
}