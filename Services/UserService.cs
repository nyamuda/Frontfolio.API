using Frontfolio.API.Data;
using Frontfolio.API.Dtos.Auth;
using Microsoft.EntityFrameworkCore;


public class UserService:IUserService
{

    private readonly ApplicationDbContext _context;


    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<UserDto> GetAsync(int id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(id));

        if (user == null) throw new KeyNotFoundException($@"User with ID ""{id}"" does not exist");

        return UserDto.MapFrom(user);
    }

   
}

