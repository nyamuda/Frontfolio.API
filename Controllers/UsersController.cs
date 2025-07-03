using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;
    private readonly JwtService _jwtService;


    public UsersController(UserService userService, JwtService jwtService)
    {
        _userService = userService;
        _jwtService = jwtService;
    }

    //Get user by ID
    [HttpGet("{id}", Name ="GetUserById")]
    [Authorize]
    public async Task<IActionResult> Get(int id)
    {
        try
        {
            //get the JWT
            var accessToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            //validate the token 
            ClaimsPrincipal claimsPrincipal = _jwtService.ValidateJwtToken(accessToken);
            //The ID of the user from the token
            string tokenUserId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier) 
                ?? throw new KeyNotFoundException("Access token lacks the name identifier claim.");
             
            //For a user to access this resource
            //the ID from their access token must match the ID of the user they're trying to access
            var user = await _userService.GetAsync(id);
            if (!user.Id.ToString().Equals(tokenUserId)) return Forbid(ErrorMessageHelper.ForbiddenErrorMessage());

            return Ok(user);

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

