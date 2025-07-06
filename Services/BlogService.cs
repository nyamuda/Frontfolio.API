
using Frontfolio.API.Data;
using Microsoft.EntityFrameworkCore;

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


}

