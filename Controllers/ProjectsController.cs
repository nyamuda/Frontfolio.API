using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


[Route("api/[controller]")]
[ApiController]
public class ProjectsController : ControllerBase
{
    private readonly ProjectService _projectService;
    private readonly JwtService _jwtService;

    public ProjectsController(ProjectService projectService, JwtService jwtService)
    {
        _projectService = projectService; 
        _jwtService = jwtService;
    }


    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> Get(int id)
    {
        try
        {

        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });

        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });

        }
        catch (Exception)
        {

        }

    }
}
