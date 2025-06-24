
    public class PageInfo<T>
    {
    public required int Page { get; set; }

    public required int PageSize { get; set; }

    public bool HasMore { get; set; }

    public List<T> Items { get; set; } = [];
    }
