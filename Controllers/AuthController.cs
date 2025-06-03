using Frontfolio.API.Dtos.Auth;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860


[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly OtpService _otpService;
    private readonly JwtService _jwtService;

    public AuthController(AuthService authService, OtpService otpService,JwtService jwtService)
    {
        _authService = authService;
        _otpService = otpService;
        _jwtService = jwtService;
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

            return Ok(token);

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

    public async Task<IActionResult> PasswordResetVerifyOtp(OtpVerificationDto otpVerificationDto)
    {
        try
        {
          

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
}

