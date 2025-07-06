
    public interface IBlogService
    {
    Task<BlogDto> GetAsync(int blogId, int tokenUserId);

    Task<PageInfo<BlogDto>> GetAllAsync(int page, int pageSize, int userId, BlogSortOption sortOption, BlogStatusFilter filterOption);

    Task<BlogDto> CreateAsync(int userId, AddBlogDto blogDto);

    Task UpdateAsync(int blogId, int tokenUserId, UpdateBlogDto updateBlogDto);

    Task DeleteAsync(int blogId, int tokenUserId);

}

