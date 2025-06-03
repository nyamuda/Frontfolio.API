
using Frontfolio.API.Data;
using System.Security.Cryptography;

public class OtpService
    {

    public ApplicationDbContext _context;


    public OtpService(ApplicationDbContext context)
    {
        _context = context;
    }


    //Generate a secure six figure random one time password (OTP)
    public string GenerateRandomOtp()
    {

        // Generate a cryptographically secure random integer number between 0 and 999 999
        int randomNumber = RandomNumberGenerator.GetInt32(0, 1_000_000);

        //Make sure the OTP always has six digits
        string optValue = randomNumber.ToString("D6");

        return optValue;

    }
}

