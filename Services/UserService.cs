using Frontfolio.API.Data;
using Frontfolio.API.Dtos.Auth;
using Microsoft.EntityFrameworkCore;

namespace Frontfolio.API.Services
{
    public class UserService
    {

        private readonly ApplicationDbContext _context;


        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        
        public async Task<UserDto> GetUserById(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(id));

            if (user == null) throw new KeyNotFoundException($@"User with ID ""{id}"" does not exist");

            return UserDto.MapFrom(user);
        }

        public async Task<string> GenerateUserOtp(string email)
        {
            //first, get the user(
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
            if (user is null) throw new KeyNotFoundException($@"User with email ""{email}"" does not exist.");


        }
    }
}
