// DTO yêu cầu phân trang cho Club
public class ClubPagedRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    // Có thể bổ sung thêm các trường filter nếu cần, ví dụ: public string? Keyword { get; set; }
}