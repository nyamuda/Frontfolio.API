
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
    private readonly OtpService _otpService;

    public AuthService(
        ApplicationDbContext context,
        JwtService jwtService,
        EmailSenderService emailSenderService,
        HtmlTemplateService htmlTemplateService,
        IConfiguration config,
        OtpService otpService
        )
    {
        _context = context;
        _jwtService = jwtService;
        _emailSenderService = emailSenderService;
        _htmlTemplateService = htmlTemplateService;
        _otpService = otpService;

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


  



    /// <summary>
    /// Handles an email verification request by generating a secure OTP, sending it to the user via email,
    /// and saving the hashed OTP in the database.
    /// </summary>
    /// <param name="emailVerificationDto">The DTO containing the user's email address.</param>
    /// <remarks>
    /// The OTP is valid for 10 minutes and is securely hashed before being stored.
    /// </remarks>
    public async Task EmailConfirmationRequest(EmailVerificationRequestDto emailVerificationDto)
    {

        //first, generate a random six OTP to send to the user
        string optValue = _otpService.GenerateRandomOtp();

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
    public async Task VerifyEmailUsingOtp(OtpVerificationDto otpVerificationDto)
    {

        //verify the provided OTP
        await _otpService.VerifyUserOtp(otpVerificationDto);

        //If we're able to get here, then the provided OTP code is valid.  
        //Check if user with given email exists
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(otpVerificationDto.Email));
        if (user is null)
            throw new KeyNotFoundException($@"User with email ""{otpVerificationDto.Email}"" does not exist.");

        //Finally, mark the user as verified
        user.isVerified = true;
        await _context.SaveChangesAsync();

    }



    /// <summary>
    /// Handles a password reset request by generating a secure OTP, sending it to the user via email,
    /// and saving the hashed OTP in the database.
    /// </summary>
    /// <param name="resetRequestDto">The DTO containing the user's email address.</param>
    /// <remarks>
    /// The OTP is valid for 10 minutes and is securely hashed before being stored.
    /// </remarks>
    public async Task PasswordResetRequest(PasswordResetRequestDto resetRequestDto)
    {

        //first, generate a random six OTP to send to the user
        string optValue =_otpService.GenerateRandomOtp();

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

    /// <summary>
    /// Resets a user's password after validating the reset token.
    /// </summary>
    /// <param name="resetPasswordDto">The DTO containing the new password and the reset token.</param>
    /// <remarks>
    /// This method securely hashes the new password using BCrypt and updates the user's record.
    /// </remarks>
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

    /// <summary>
    /// Verifies the OTP and generates a secure JWT token for password reset if the OTP is valid.
    /// </summary>
    /// <param name="otpVerificationDto">DTO containing the user's email and OTP.</param>
    /// <returns>A JWT token that can be used to reset the user's password.</returns>
    public async Task<string> VerifyOtpAndGenerateResetToken(OtpVerificationDto otpVerificationDto)
    {
        //verify OTP
        await _otpService.VerifyUserOtp(otpVerificationDto);

        //If the OTP is valid,
        //generate the password reset token and send to the client
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(otpVerificationDto.Email));
        if (user is null)    
            throw new KeyNotFoundException($@"User with email ""{otpVerificationDto.Email}"" does not exist.");

        var resetToken = await _jwtService.GenerateJwtToken(userId: user.Id);

        return resetToken;
    }


}
