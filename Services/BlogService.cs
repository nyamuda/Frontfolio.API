
using Frontfolio.API.Data;

public class BlogService:IBlogService
    {

    private readonly ApplicationDbContext _context;

    public BlogService(ApplicationDbContext context) { 
        _context = context;
    }

    public async Task<BlogDto> GetAsync(int blogId, int tokenUserId)
    {

    }


    }

