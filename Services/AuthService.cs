
using Frontfolio.API.Data;
using Frontfolio.API.Dtos.Auth;
using Microsoft.EntityFrameworkCore;
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
       bool doesUserExist = await _context.Users.AnyAsync(u => u.Email.Equals(addUserDto.Email));
        if (doesUserExist) throw new InvalidOperationException("An account with this email already exists.");


        //hash the password
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(addUserDto.Password);
        addUserDto.Password = hashedPassword;

        //create user instance
        User user = new()
        {
            Name = addUserDto.Name,
            Email = addUserDto.Email,
            Password = addUserDto.Password
        };

        //save the new user
        _context.Users.Add(user);
        await _context.SaveChangesAsync();


        return User


    }
}
