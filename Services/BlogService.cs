
using Frontfolio.API.Data;
using Frontfolio.API.Models;
using Frontfolio.API.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

public class BlogService : IBlogService
{

    private readonly ApplicationDbContext _context;
    private readonly ParagraphService _paragraphService;

    public BlogService(ApplicationDbContext context,Paragraph)
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


    public async Task UpdateAsync(int blogId, int tokenUserId, UpdateBlogDto updateBlogDto)
    {
        //check if user with the given ID exist
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(tokenUserId))
            ?? throw new KeyNotFoundException($@"User with ID ""{tokenUserId}"" does not exist.");

        //map UpdateBlogDto to Blog
        Blog blog = UpdateBlogDto.MapTo(updateBlogDto);

        //get the existing blog
        Blog existingBlog = await _context.Blogs.FirstOrDefaultAsync(p => p.Id.Equals(blogId))
            ?? throw new KeyNotFoundException(($@"Blog with ID ""{blogId}"" does not exist."));

        //Only the owner the blog is allowed to update it
        BlogHelper.EnsureUserOwnsBlog(tokenUserId, existingBlog, crudContext: CrudContext.Update);

        // Update all properties
        existingBlog.Title = blog.Title;
        existingBlog.Status = blog.Status;
        existingBlog.SortOrder = blog.SortOrder;
        existingBlog.DifficultyLevel = blog.DifficultyLevel;
        existingBlog.StartDate = blog.StartDate;
        existingBlog.EndDate = blog.EndDate;
        existingBlog.Summary = blog.Summary;
        existingBlog.GitHubUrl = blog.GitHubUrl;
        existingBlog.ImageUrl = blog.ImageUrl;
        existingBlog.VideoUrl = blog.VideoUrl;
        existingBlog.LiveUrl = blog.LiveUrl;
        existingBlog.TechStack = blog.TechStack;
        existingBlog.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        //STEP 1. Update the nested background paragraphs
        //The updated background paragraph list contains the updated paragraphs as well as some new ones
        //add the new paragraphs
        await _paragraphService.AddIfNotExistingAsync(existingBlog.Id, blog.Background);
        //update existing ones
        await _paragraphService.UpdateExistingAsync(existingBlog.Id, blog.Background);

        //STEP 2. Update the nested challenges
        //The updated challenge list contains the updated challenges as well as some new ones
        //add the new challenges
        await _challengeService.AddIfNotExistingAsync(existingBlog.Id, blog.Challenges);
        //update existing ones
        await _challengeService.UpdateExistingAsync(existingBlog.Id, blog.Challenges);

        //STEP 3. Update the nested achievements
        //The updated achievement list contains the updated achievements as well as some new ones
        //add the new achievements
        await _achievementService.AddIfNotExistingAsync(existingBlog.Id, blog.Achievements);
        //update existing ones
        await _achievementService.UpdateExistingAsync(existingBlog.Id, blog.Achievements);

        //STEP 4. Update the nested feedback items
        //The updated feedback list contains the updated feedback items as well as new ones
        //add the new feedback items
        await _feedbackService.AddIfNotExistingAsync(existingBlog.Id, blog.Feedback);
        //update existing ones
        await _feedbackService.UpdateExistingAsync(existingBlog.Id, blog.Feedback);


    }

}

