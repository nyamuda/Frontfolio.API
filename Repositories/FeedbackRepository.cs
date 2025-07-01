using Frontfolio.API.Data;
using Frontfolio.API.Repositories.Abstractions;

namespace Frontfolio.API.Repositories
{
    public class FeedbackRepository:IFeedbackRepository
    {
        private readonly ApplicationDbContext _context;

        public FeedbackRepository(ApplicationDbContext context)
        {
            _context = context;
        }

       
    }
}
