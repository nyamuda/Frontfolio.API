
using Frontfolio.API.Data;
using Frontfolio.API.Models;
using Frontfolio.API.Services;
using Frontfolio.API.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

public class BlogService : IBlogService
{

    private readonly ApplicationDbContext _context;
    private readonly BlogParagraphService _paragraphService;

    public BlogService(ApplicationDbContext context,BlogParagraphService paragraphService)
    {
        _context = context;
        _paragraphService = paragraphService;
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

    //Add a new blog
    public async Task<BlogDto> CreateAsync(int userId, AddBlogDto addBlogDto)
    {
        //check if user with the given ID exist
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(userId))
        ?? throw new KeyNotFoundException($@"User with ID ""{userId}"" does not exist.");

        //Map AddBlogDto to Blog so that we can save the blog to the database
        Blog newBlog = AddBlogDto.MapTo(addBlogDto);
        //add the user ID
        newBlog.UserId = userId;

        //save the new blog
        _context.Blogs.Add(newBlog);
        await _context.SaveChangesAsync();

        return BlogDto.MapFrom(newBlog);
    }

    //Update existing blog
    public async Task UpdateAsync(int blogId, int tokenUserId, UpdateBlogDto updateBlogDto)
    {

        //get the existing blog
        Blog existingBlog = await _context.Blogs.FirstOrDefaultAsync(p => p.Id.Equals(blogId))
            ?? throw new KeyNotFoundException(($@"Blog with ID ""{blogId}"" does not exist."));

        //Only the owner the blog is allowed to update it
        BlogHelper.EnsureUserOwnsBlog(tokenUserId, existingBlog, crudContext: CrudContext.Update);


        //map UpdateBlogDto to Blog
        Blog updatedBlog = UpdateBlogDto.MapTo(updateBlogDto);

        // Update properties
        existingBlog.Title = updatedBlog.Title;
        existingBlog.Topic = updatedBlog.Topic;
        existingBlog.Summary = updatedBlog.Summary;
        existingBlog.Content = updatedBlog.Content;
        existingBlog.ImageUrl = updatedBlog.ImageUrl;
        existingBlog.Tags= updatedBlog.Tags;
        existingBlog.UpdatedAt = updatedBlog.UpdatedAt;
      
        await _context.SaveChangesAsync();

        //Final Step: Update the nested content paragraphs
        //The updated content paragraph list contains the updated paragraphs as well as some new ones
        //add the new paragraphs
        await _paragraphService.AddIfNotExistingAsync(existingBlog.Id, updatedBlog.Content);
        //update existing ones
        await _paragraphService.UpdateExistingAsync(existingBlog.Id, updatedBlog.Content);

    }

     //Delete a blog
    public async Task DeleteAsync(int blogId,int tokenUserId)
    {
        var blog = await _context.Blogs.FirstOrDefaultAsync(p => p.Id.Equals(blogId))
            ?? throw new KeyNotFoundException($@"Blog with ID ""{blogId}"" does not exist.");

        //Only the owner the blog is allowed to delete it
        BlogHelper.EnsureUserOwnsBlog(tokenUserId, blog, crudContext: CrudContext.Delete);

        _context.Blogs.Remove(blog);
        await _context.SaveChangesAsync();
    }

    //Publish a blog
    public async Task PublishAsync(int blogId, int tokenUserId)
    {
        //get the blog to be published
        var blog = await _context.Blogs.FirstOrDefaultAsync(p => p.Id.Equals(blogId))
            ?? throw new KeyNotFoundException($@"Blog with ID ""{blogId}"" does not exist.");

        //Only the owner the blog is allowed to publish it
        BlogHelper.EnsureUserOwnsBlog(tokenUserId, blog, crudContext: CrudContext.Update);

        //check if blog is not already published
        if (blog.Status.Equals(BlogStatus.Published))
            throw new InvalidOperationException("Cannot publish: the blog is already marked as published.");

        //publish blog
        blog.Status = BlogStatus.Published;
        blog.PublishedAt= DateTime.UtcNow;

        await _context.SaveChangesAsync();

    }

}

