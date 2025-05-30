using Frontfolio.API.Data;
using Frontfolio.API.Dtos.Auth;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;


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

    //Generate a secure random one time password (OTP)
    public string GenerateRandomOtp()
    {

        // Generate a cryptographically secure random integer number between 0 and 999 999
        int randomNumber = RandomNumberGenerator.GetInt32(0, 1_000_000);

        //Make sure the OTP always has six digits
        string optValue = randomNumber.ToString("D6");

        return optValue;

    }
}

