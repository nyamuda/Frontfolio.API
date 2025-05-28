
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

        return UserDto.MapFrom(user);

    }


    public async Task<string> Login(LoginUserDto loginUserDto)
    {
        //check if the user with given email exists
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(loginUserDto.Email));
        if (user is null) throw new KeyNotFoundException("User with the provided email does not exist.");

        //check if the provided password is correct
        bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(loginUserDto.Password,user.Password);
        if (!isPasswordCorrect) throw new UnauthorizedAccessException("The provided password is incorrect.");

        //if we're here, then everything is good
        //create an access token
        //access token lifespan is 72 hours  = 4320 minutes
        double lifespanInMinutes = 4320;
        string accessToken = await _jwtService.GenerateJwtToken(userId: user.Id, expirationInMinutes: lifespanInMinutes);

        return accessToken;
    }
}
