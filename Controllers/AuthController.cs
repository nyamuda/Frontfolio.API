using Frontfolio.API.Dtos.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860


[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly OtpService _otpService;
    private readonly JwtService _jwtService;
    private readonly UserService _userService;

    public AuthController(AuthService authService, OtpService otpService,JwtService jwtService, UserService userService)
    {
        _authService = authService;
        _otpService = otpService;
        _jwtService = jwtService;
        _userService = userService;
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register(AddUserDto addUserDto)
    {
       
     try
        {
            var userDto = await _authService.Register(addUserDto);

            return CreatedAtRoute(routeName: "GetUserById", routeValues: new { id = userDto.Id }, value: userDto);
        }

        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }


    }

    
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserDto loginUserDto)
    {
        try
        {
            string token = await _authService.Login(loginUserDto);

            return Ok(new {token});

        }
        catch(UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            var response = new
            {
                message = ErrorMessageHelper.UnexpectedErrorMessage(),
                details = ex.Message
            };

            return StatusCode(500, response);
        }
    }

    //Send an email verification email to a user
    [HttpPost("email-verification/request")]
    public async Task<IActionResult> EmailVerificationRequest(EmailVerificationRequestDto emailVerificationRequestDto)
    {
        try
        {
            await _authService.EmailConfirmationRequest(emailVerificationRequestDto);

            return NoContent();

        }
        catch(KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            var response = new
            {
                message = ErrorMessageHelper.UnexpectedErrorMessage(),
                details = ex.Message
            };

            return StatusCode(500, response);
        }
    }


    //Verify user email using the provided OTP code
    [HttpPost("email-verification/verify")]
    public async Task<IActionResult> VerifyEmailWithOptCode(OtpVerificationDto verificationDto)
    {
        try
        {
            await _authService.VerifyEmailUsingOtp(verificationDto);

            return NoContent();

        }
        catch(InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            var response = new { message = ErrorMessageHelper.UnexpectedErrorMessage(), details = ex.Message };

            return StatusCode(500, response);

        }
    }
    //Send a request to reset password
    [HttpPost("password-reset/request")]
    public async Task<IActionResult> PasswordResetRequest(PasswordResetRequestDto resetRequestDto)
    {
        try
        {
            await _authService.PasswordResetRequest(resetRequestDto);

            return NoContent();

        }
        catch(KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            var response = new
            {
                message = ErrorMessageHelper.UnexpectedErrorMessage(),
                details = ex.Message
            };
            return StatusCode(500, response);
        }
    }

    //Verifies the OTP and generates a secure JWT token for password reset if the OTP is valid.
    [HttpPost("password-reset/verify-otp")]
    public async Task<IActionResult> ValidateOtpAndGenerateResetJwt(OtpVerificationDto otpVerificationDto)
    {
        try
        {
            var resetToken = await _authService.VerifyOtpAndGenerateResetToken(otpVerificationDto);

            return Ok(new { resetToken });

        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            var response = new
            {
                message = ErrorMessageHelper.UnexpectedErrorMessage(),
                details = ex.Message
            };
            return StatusCode(500, response);
        }
    }
    //Reset user password using reset token
    [HttpPatch("password-reset/reset")]
    public async Task<IActionResult> ResetUserPassword(ResetPasswordDto resetPasswordDto)
    {
        try
        {
            await _authService.ResetPassword(resetPasswordDto);

            return NoContent();

        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            var response = new
            {
                message = ErrorMessageHelper.UnexpectedErrorMessage(),
                details = ex.Message
            };
            return StatusCode(500, response);
        }

    }


    //Get user details using their token
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMe()
    {
        try
        {
            //get the JWT
            var accessToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            //validate the token 
            ClaimsPrincipal claimsPrincipal = _jwtService.ValidateJwtToken(accessToken);
            //The ID of the user from the token
            string tokenUserId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new KeyNotFoundException(ErrorMessageHelper.InvalidNameIdentifierMessage());

            //Get the user info using the token userId claim
            if(int.TryParse(tokenUserId,out int userId)) {
                var user = await _userService.GetAsync(userId);
                return Ok(user);
            }

            //throw an exception if tokenUserId cannot be parsed
            throw new UnauthorizedAccessException(ErrorMessageHelper.InvalidNameIdentifierMessage());

        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            var response = new
            {
                message = ErrorMessageHelper.UnexpectedErrorMessage(),
                details = ex.Message
            };

            return StatusCode(500, response);

        }
    }
}

