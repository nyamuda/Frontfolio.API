using Frontfolio.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly ProjectService _projectService;
    private readonly JwtService _jwtService;
    private readonly ParagraphService _paragraphService;

    public ProjectsController(ProjectService projectService, JwtService jwtService,ParagraphService paragraphService)
    {
        _projectService = projectService;
        _jwtService = jwtService;
        _paragraphService = paragraphService;

    }



    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        try
        {
            //First, extract the user's access token from the Authorization header
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            //Manually validate the token and then grap the User ID from the token claims
            ClaimsPrincipal claims = _jwtService.ValidateJwtToken(token);
            string tokenUserId = claims.FindFirstValue(ClaimTypes.NameIdentifier) ??
                throw new UnauthorizedAccessException("Access denied. Token does not contain a valid user ID claim.");


            //Get the project
            var project = await _projectService.GetProjectById(id);

            // Compare the User ID from the token with the User ID of the project
            // A user is only allowed to access their own projects.
            // If the user ID from the token does not match the project owner's ID, deny access.
            if (int.TryParse(tokenUserId, out int userId))
            {
                if (project.UserId != userId)
                    return Forbid("You don't have permission to view this project.");

                return Ok(project);
            }

            //throw an exception if tokenUserId cannot be parsed
            throw new UnauthorizedAccessException("Access denied. Token does not contain a valid user ID claim.");
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


    [HttpGet]
    public async Task<IActionResult> GetProjects(ProjectSortOption? sortBy, int page = 1, int pageSize = 5)
    {
        try
        {
            //First, extract the user's access token from the Authorization header
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            //Manually validate the token and then grab the User ID from the token claims
            ClaimsPrincipal claims = _jwtService.ValidateJwtToken(token);
            string tokenUserId = claims.FindFirstValue(ClaimTypes.NameIdentifier) ??
                throw new UnauthorizedAccessException("Access denied. Token does not contain a valid user ID claim.");

            //Get the paginated projects for a user with the given ID
            if (int.TryParse(tokenUserId, out int userId))
            {
                PageInfo<ProjectDto> paginatedProjects = await _projectService
                .GetProjects(page: page, pageSize: pageSize, userId: userId, sortOption: sortBy);

                return Ok(paginatedProjects);
            }
            //throw an exception if tokenUserId cannot be parsed
            throw new UnauthorizedAccessException("Access denied. Token does not contain a valid user ID claim.");

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
    [HttpPost]
    public async Task<IActionResult> Post(AddProjectDto addProjectDto)
    {
        try
        {
            //First, extract the user's access token from the Authorization header
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            //Manually validate the token and then grab the User ID from the token claims
            ClaimsPrincipal claims = _jwtService.ValidateJwtToken(token);
            string tokenUserId = claims.FindFirstValue(ClaimTypes.NameIdentifier) ??
                throw new UnauthorizedAccessException("Access denied. Token does not contain a valid user ID claim.");

           if(int.TryParse(tokenUserId,out int userId))
            {
                //Add the new project
                var project = await _projectService.AddProject(userId, addProjectDto);

                return CreatedAtAction(nameof(Get), new { id = project.Id }, project);
            }

            //throw an exception if tokenUserId cannot be parsed
            throw new UnauthorizedAccessException("Access denied. Token does not contain a valid user ID claim.");

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

    //Update a project
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateProjectDto updateProjectDto)
    {
        try
        {
            //First, extract the user's access token from the Authorization header
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            //Manually validate the token and then grab the User ID from the token claims
            ClaimsPrincipal claims = _jwtService.ValidateJwtToken(token);
            string tokenUserId = claims.FindFirstValue(ClaimTypes.NameIdentifier) ??
                throw new UnauthorizedAccessException("Access denied. Token does not contain a valid user ID claim.");

            // Compare the User ID from the token with the User ID of the project to be updated
            // A user is only allowed to update their own projects.
            // If the user ID from the token does not match the project owner's ID, deny access.
           if(int.TryParse(tokenUserId,out int userId))
            {
                var oldProject = await _projectService.GetProjectById(id);
                if (!oldProject.UserId.Equals(userId))
                    return Forbid("You don't have permission to update this project.");

                //update project
                await _projectService.UpdateProject(userId: userId, projectId: id, updateProjectDto);

                return NoContent();
            }

            //throw an exception if tokenUserId cannot be parsed
            throw new UnauthorizedAccessException("Access denied. Token does not contain a valid user ID claim.");

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
                details = ex.ToString()
            };

            return StatusCode(500, response);
        }
    }

    //Delete a project
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            //First, extract the user's access token from the Authorization header
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            //Manually validate the token and then grab the User ID from the token claims
            ClaimsPrincipal claims = _jwtService.ValidateJwtToken(token);
            string tokenUserId = claims.FindFirstValue(ClaimTypes.NameIdentifier) ??
                throw new UnauthorizedAccessException("Access denied. Token does not contain a valid user ID claim.");

            // Compare the User ID from the token with the User ID of the project to be deleted
            // A user is only allowed to delete their own projects.
            // If the user ID from the token does not match the project owner's ID, deny access.
            if (int.TryParse(tokenUserId, out int userId))
            {
                var oldProject = await _projectService.GetProjectById(id);
                if (!oldProject.UserId.Equals(userId))
                    return Forbid("You don't have permission to delete this project.");

                //delete project
                await _projectService.DeleteProject(id);

                return NoContent();
            }

            //throw an exception if tokenUserId cannot be parsed
            throw new UnauthorizedAccessException("Access denied. Token does not contain a valid user ID claim.");

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
    //Add a background paragraph for a project with a given ID
    [HttpPost("{projectId}/backgrounds")]
    public async Task<IActionResult> AddBackgroundParagraph(int projectId,AddParagraphDto paragraphDto)
    {
        try
        {
            //Retrieve the access token from the request
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            //Manually validate the token
            ClaimsPrincipal claims =_jwtService.ValidateJwtToken(token);
            //Get the user ID claim from the token
            string tokenUserId = claims.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("Access denied. Token does not contain a valid user ID claim.");

            if(int.TryParse(tokenUserId, out int userId))
            {
                await _paragraphService
                    .AddProjectBackgroundParagraph(projectId,userId, paragraphDto);

                return StatusCode(201);
            }
            //throw an exception if tokenUserId cannot be parsed
            throw new UnauthorizedAccessException("Access denied. Token does not contain a valid user ID claim.");

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

    //Update background paragraph for a specific project
    [HttpPut("{projectId}/backgrounds/{backgroundId}")]
    public async Task<IActionResult> UpdateBackgroundParagraph(int projectId,int backgroundId,UpdateParagraphDto paragraphDto)
    {
        try
        {
            //Retrieve the access token from the request
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            //Manually validate the token
            ClaimsPrincipal claims = _jwtService.ValidateJwtToken(token);
            //Get the user ID claim from the token
            string tokenUserId = claims.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("Access denied. Token does not contain a valid user ID claim.");

            if (int.TryParse(tokenUserId, out int userId))
            {
                await _paragraphService
                    .UpdateProjectBackgroundParagraph(projectId, backgroundId, userId, paragraphDto);

                return NoContent();
            }
            //throw an exception if tokenUserId cannot be parsed
            throw new UnauthorizedAccessException("Access denied. Token does not contain a valid user ID claim.");

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
