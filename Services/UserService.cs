using Frontfolio.API.Data;

namespace Frontfolio.API.Services
{
    public class UserService
    {

        public readonly ApplicationDbContext _context;


        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
