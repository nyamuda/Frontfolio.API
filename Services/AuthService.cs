
using Frontfolio.API.Data;
using Frontfolio.API.Dtos.Auth;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
public class AuthService
{
    private readonly ApplicationDbContext _context;
    private readonly JwtService _jwtService;
    private readonly EmailSenderService _emailSenderService;
    private readonly HtmlTemplateService _htmlTemplateService;
    private readonly string _appName;

    public AuthService(
        ApplicationDbContext context,
        JwtService jwtService,
        EmailSenderService emailSenderService,
        HtmlTemplateService htmlTemplateService,
        IConfiguration config
        )
    {
        _context = context;
        _jwtService = jwtService;
        _emailSenderService = emailSenderService;
        _htmlTemplateService = htmlTemplateService;

        //App name from configuration setting
        _appName = config.GetValue<string>("AppSettings:AppName")
            ?? throw new KeyNotFoundException("App name not found in config.");

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
        if (user is null) throw new KeyNotFoundException($@"User with email ""{loginUserDto.Email}"" does not exist.");

        //check if the provided password is correct
        bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(loginUserDto.Password, user.Password);
        if (!isPasswordCorrect) throw new UnauthorizedAccessException("The provided password is incorrect.");

        //if we're here, then everything is good
        //create an access token
        //access token lifespan is 72 hours  = 4320 minutes
        double lifespanInMinutes = 4320;
        string accessToken = await _jwtService.GenerateJwtToken(userId: user.Id, expirationInMinutes: lifespanInMinutes);

        return accessToken;
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


    //Make a request to verify email address
    public async Task EmailConfirmationRequest(EmailVerificationRequestDto emailVerificationDto)
    {

        //first, generate a random six OTP to send to the user
        string optValue = GenerateRandomOtp();

        //get user details
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(emailVerificationDto.Email));
        if (user is null)
            throw new KeyNotFoundException($@"User with email ""{emailVerificationDto.Email}"" does not exist.");

        //HTML template for email verification
        var emailHtmlTemplate = _htmlTemplateService.EmailConfirmationTemplate(optValue, user.Name, _appName);
        //send email to user
        await _emailSenderService.SendEmail(user.Name, user.Email, "Email Verification", emailHtmlTemplate);

        //Once the email is sent, save the OTP to the database
        ///Hash the OTP
        var hashedOpt = BCrypt.Net.BCrypt.HashPassword(optValue);
        UserOtp userOtp = new()
        {
            Email = user.Email,
            OtpCode = hashedOpt,
            ExpirationTime = DateTime.UtcNow.AddMinutes(10),
            IsUsed = false,
            UserId = user.Id

        };

        _context.UserOtps.Add(userOtp);

        await _context.SaveChangesAsync();

    }
    /// <summary>
    /// Verifies a user's email address by checking the provided OTP code.
    /// </summary>
    /// <param name="otpVerificationDto">The DTO containing the user's email and OTP code.</param>
    public async Task VerifyEmailUsingOtpCode(OtpVerificationDto otpVerificationDto)
    {
        //check if user with given email exists
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(otpVerificationDto.Email));
        if (user is null)
            throw new KeyNotFoundException($@"User with email ""{otpVerificationDto.Email}"" does not exist.");

        // Retrieve the most recent valid OTP that hasn't expired or been used
        var userOpt = await _context.UserOtps
            .Where(o => o.Email.Equals(user.Email) &&
            o.ExpirationTime > DateTime.UtcNow &&
            !o.IsUsed
            )
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync();

        if (userOpt is null) throw new InvalidOperationException("No valid or active OTP found for this email.");

        //The saved OTP code is hashed
        //Check if the provided OTP matched the saved one
        bool isOptCorrect = BCrypt.Net.BCrypt.Verify(otpVerificationDto.OtpCode, userOpt.OtpCode);
        if(!isOptCorrect) throw new UnauthorizedAccessException("Invalid OTP. Please check the code and try again.");

        //If we're able to get here, then the provided OTP code is valid
        //Finally, mark the user as verified and the OPT as used
        user.isVerified = true;
        userOpt.IsUsed = true;

        await _context.SaveChangesAsync();

    }



    //Make a request to reset password
    public async Task PasswordResetRequest(PasswordResetRequestDto resetRequestDto)
    {

        //first, generate a random six OTP to send to the user
        string optValue = GenerateRandomOtp();

        //get user details
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(resetRequestDto.Email));
        if (user is null)
            throw new KeyNotFoundException($@"User with email ""{resetRequestDto.Email}"" does not exist.");

        //HTML template for password rest
        var emailHtmlTemplate = _htmlTemplateService.PasswordResetTemplate(optValue, user.Name, _appName);
        //send email to user
        await _emailSenderService.SendEmail(user.Name, user.Email, "Password Reset", emailHtmlTemplate);

        //Once the email is sent, save the OTP to the database
        ///Hash the OTP
        var hashedOpt = BCrypt.Net.BCrypt.HashPassword(optValue);
        UserOtp userOtp = new()
        {
            Email = user.Email,
            OtpCode = hashedOpt,
            ExpirationTime = DateTime.UtcNow.AddMinutes(10),
            IsUsed = false,
            UserId = user.Id

        };

        _context.UserOtps.Add(userOtp);

        await _context.SaveChangesAsync();

    }

    //Reset the password of a user
    public async Task ResetPassword(ResetPasswordDto resetPasswordDto)
    {
        //first, validate the password reset token
        ClaimsPrincipal claims = _jwtService.ValidateJwtToken(resetPasswordDto.ResetToken);

        //once the token is validated,
        //grab the user ID and get the details of the user using that ID
        string userId = claims.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new KeyNotFoundException("Password reset token is missing name identifier claim.");

        User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(int.Parse(userId)));

        if (user is null) throw new KeyNotFoundException($"User with ID {userId} does not exist.");

        //hash the password
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(resetPasswordDto.Password);

        //save the new password
        user.Password = hashedPassword;

        await _context.SaveChangesAsync();
    }


}
