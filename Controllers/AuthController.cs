using Frontfolio.API.Dtos.Auth;
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
        var userDto = await _authService.Register(addUserDto);

        return CreatedAtAction()
        }

    // GET api/<AuthController>/5
    [HttpGet("users/{id}")]
    public string GetUserById(int id)
    {
        return "value";
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

