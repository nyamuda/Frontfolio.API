using Frontfolio.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Frontfolio.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;


        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        //Get user by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {

            }
            catch (KeyNotFoundException ex)
            {

            }
            catch (Exception ex)
            {

            }
        }
    }
}
