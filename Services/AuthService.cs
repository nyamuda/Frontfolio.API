
using Frontfolio.API.Data;
using Frontfolio.API.Dtos;

public class AuthService
{
    private readonly ApplicationDbContext _context;
    private readonly JwtService _jwtService;

    public AuthService(ApplicationDbContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;

    }

    public async Task<UserDto> Register(AddUserDto addUserDto)
    {
        //first check if there aren't any users with registered with the provided email

    }
}
