
using Frontfolio.API.Data;
using Microsoft.EntityFrameworkCore;
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

    /// <summary>
    /// Verifies a one-time password (OTP) submitted by the user by checking against the most recent active and unused OTP.
    /// </summary>
    /// <param name="otpVerificationDto">The DTO containing the user's email and the OTP to verify.</param>
    /// <exception cref="KeyNotFoundException">Thrown if no user is found with the provided email.</exception>
    /// <exception cref="InvalidOperationException">Thrown if no valid OTP is found (expired or already used).</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown if the provided OTP does not match the saved code.</exception>
    /// <remarks>
    /// This method ensures that only the latest unused and unexpired OTP is checked. Stored OTPs are hashed for security.
    /// </remarks>
    public async Task<UserOtp> VerifyUserOtp(OtpVerificationDto otpVerificationDto)
    {
        //check if user with given email exists
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(otpVerificationDto.Email));
        if (user is null)
            throw new KeyNotFoundException($@"User with email ""{otpVerificationDto.Email}"" does not exist.");

        // Retrieve the most recent valid OTP that hasn't expired or been used
        var userOtp = await _context.UserOtps
            .Where(o => o.Email.Equals(user.Email) &&
            o.ExpirationTime > DateTime.UtcNow &&
            !o.IsUsed
            )
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync();

        if (userOtp is null) throw new InvalidOperationException("No valid or active OTP found for this email.");

        //The saved OTP code is hashed
        //Check if the provided OTP matched the saved one
        bool isOptCorrect = BCrypt.Net.BCrypt.Verify(otpVerificationDto.OtpCode, userOpt.OtpCode);
        if (!isOptCorrect) throw new UnauthorizedAccessException("Invalid OTP. Please check the code and try again.");

        //return the user OTP
        return userOtp;
    }
}

