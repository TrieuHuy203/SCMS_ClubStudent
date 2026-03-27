
// phân trang

 public class PagedResult<T> // T là kiểu dữ liệu của các item trong danh sách có thể dùng chung được
{
    public List<T> Items { get; set; }
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}