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
        public async
    }
}
