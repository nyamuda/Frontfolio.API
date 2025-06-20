namespace Frontfolio.API.Interfaces
{
    public interface IPageInfo
    {
        int Page { get; set; }
        int PageSize { get; set; }
        bool HasMore { get; set; }
    }
}
