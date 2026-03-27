public class ConfirmEmailRequest
{
public string Email { get; set; } // Email của user cần xác nhận
public string Token { get; set; } // Token xác nhận email
}