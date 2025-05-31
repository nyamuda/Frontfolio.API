using Frontfolio.API.Dtos.Auth;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860


[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    // GET: api/<AuthController>
    [HttpGet("register")]
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

    // GET api/<AuthController>/5
    [HttpGet("login")]
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

    // POST api/<AuthController>
    [HttpPost]
    public void Post([FromBody] string value)
    {
    }

    // PUT api/<AuthController>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/<AuthController>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}

