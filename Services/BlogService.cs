
using Frontfolio.API.Data;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

public class BlogService : IBlogService
{

    private readonly ApplicationDbContext _context;

    public BlogService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BlogDto> GetAsync(int blogId, int tokenUserId)
    {
        var blog = await _context.Blogs
            .Include(b => b.Content)
            .AsSplitQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id.Equals(blogId))
            ?? throw new KeyNotFoundException($@"Blog with ID ""{blogId}"" does not exist.");

        //Only the owner the blog is allowed to access it
        BlogHelper.EnsureUserOwnsBlog(tokenUserId: tokenUserId, blog: blog, crudContext: CrudContext.Read);

        return BlogDto.MapFrom(blog);
    }


    public async Task<PageInfo<BlogDto>> GetAllAsync(int page, int pageSize,
        int userId, BlogSortOption sortOption, BlogStatusFilter filterOption)
    {
        var query = _context.Blogs.Where(b => b.UserId.Equals(userId)).AsQueryable();

        //filter the blogs by status
        query = filterOption switch
        {
            BlogStatusFilter.Published => query.Where(b => b.Status.Equals(BlogStatus.Published)),
            BlogStatusFilter.Draft => query.Where(b => b.Status.Equals(BlogStatus.Draft)),
            _ => query
        };

        //sort the blogs by the provided sortOption
        query = sortOption switch
        {
            BlogSortOption.Title => query.OrderByDescending(b => b.Title),      
            _ => query.OrderByDescending(p => p.PublishedAt) //default sort option is `SubmittedAt`
        };


        List<BlogDto> blogs = await query
             .Skip((page - 1) * pageSize)
             .Take(pageSize)
             .Select(b=> new BlogDto
             {
                 Id = b.Id,
                 Title = b.Title,
                 Topic = b.Topic,                
                 Summary = b.Summary,
                 Status = b.Status,
                 Tags = b.Tags,
                 ImageUrl = b.ImageUrl,
                 UserId = b.UserId,
                 CreatedAt = b.CreatedAt,
                 UpdatedAt = b.UpdatedAt,
                 PublishedAt=b.PublishedAt,
             }).ToListAsync();

        //check if there are still more blogs for the user
        int totalBlogs = await query.CountAsync();
        bool hasMore = totalBlogs > page * pageSize;

        PageInfo<BlogDto> pageInfo = new() { Page = page, PageSize = pageSize, HasMore = hasMore, Items = blogs };

        return pageInfo;

    }

}

