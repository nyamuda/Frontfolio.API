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
    private readonly ProjectParagraphService _paragraphService;
    private readonly ChallengeService _challengeService;
    private readonly AchievementService _achievementService;
    private readonly FeedbackService _feedbackService;

    public ProjectsController(ProjectService projectService, 
        JwtService jwtService, 
        ProjectParagraphService paragraphService,
        ChallengeService challengeService,
        AchievementService achievementService,
        FeedbackService feedbackService
        )
    {
        _projectService = projectService;
        _jwtService = jwtService;
        _paragraphService = paragraphService;
        _achievementService = achievementService;
        _challengeService = challengeService;
        _feedbackService = feedbackService;

    }



    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        try
        {
            //First, extract the user's access token from the Authorization header
            var token = HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");

            //Manually validate the token and then grab the User ID from the token claims
            ClaimsPrincipal claims = _jwtService.ValidateJwtToken(token);
            string tokenUserId = claims.FindFirstValue(ClaimTypes.NameIdentifier) ??
                throw new UnauthorizedAccessException(ErrorMessageHelper.InvalidNameIdentifierMessage());


            //Get the project       
            if (int.TryParse(tokenUserId, out int userId))
            {
                var project = await _projectService.GetAsync(projectId: id, tokenUserId: userId);
                return Ok(project);
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


    [HttpGet]
    public async Task<IActionResult> GetAll(ProjectSortOption? sortBy, int page = 1, int pageSize = 5)
    {
        try
        {
            //First, extract the user's access token from the Authorization header
            var token = HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");

            //Manually validate the token and then grab the User ID from the token claims
            ClaimsPrincipal claims = _jwtService.ValidateJwtToken(token);
            string tokenUserId = claims.FindFirstValue(ClaimTypes.NameIdentifier) ??
                throw new UnauthorizedAccessException(ErrorMessageHelper.InvalidNameIdentifierMessage());

            //Get the paginated projects for a user with the given ID
            if (int.TryParse(tokenUserId, out int userId))
            {
                PageInfo<ProjectDto> paginatedProjects = await _projectService
                .GetAllAsync(page: page, pageSize: pageSize, userId: userId, sortOption: sortBy);

                return Ok(paginatedProjects);
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
    [HttpPost]
    public async Task<IActionResult> Post(AddProjectDto addProjectDto)
    {
        try
        {
            //First, extract the user's access token from the Authorization header
            var token = HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");

            //Manually validate the token and then grab the User ID from the token claims
            ClaimsPrincipal claims = _jwtService.ValidateJwtToken(token);
            string tokenUserId = claims.FindFirstValue(ClaimTypes.NameIdentifier) ??
                throw new UnauthorizedAccessException(ErrorMessageHelper.InvalidNameIdentifierMessage());

            if (int.TryParse(tokenUserId, out int userId))
            {
                //Add the new project
                var project = await _projectService.CreateAsync(userId, addProjectDto);

                return CreatedAtAction(nameof(Get), new { id = project.Id }, project);
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

    //Update a project
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateProjectDto updateProjectDto)
    {
        try
        {
            //First, extract the user's access token from the Authorization header
            var token = HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");

            //Manually validate the token and then grab the User ID from the token claims
            ClaimsPrincipal claims = _jwtService.ValidateJwtToken(token);
            string tokenUserId = claims.FindFirstValue(ClaimTypes.NameIdentifier) ??
                throw new UnauthorizedAccessException(ErrorMessageHelper.InvalidNameIdentifierMessage());

            if (int.TryParse(tokenUserId, out int userId))
            {
                //update project
                await _projectService.UpdateAsync(projectId: id, tokenUserId: userId, updateProjectDto);

                return NoContent();
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
            var token = HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");

            //Manually validate the token and then grab the User ID from the token claims
            ClaimsPrincipal claims = _jwtService.ValidateJwtToken(token);
            string tokenUserId = claims.FindFirstValue(ClaimTypes.NameIdentifier) ??
                throw new UnauthorizedAccessException(ErrorMessageHelper.InvalidNameIdentifierMessage());


            if (int.TryParse(tokenUserId, out int userId))
            {
                //delete project
                await _projectService.DeleteAsync(projectId: id, tokenUserId: userId);

                return NoContent();
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

    //Delete background paragraph for a specific project
    [HttpDelete("{projectId}/backgrounds/{paragraphId}")]
    public async Task<IActionResult> DeleteProjectBackgroundParagraph(int projectId, int paragraphId)
    {
        try
        {
            //Retrieve the access token from the request
            var token = HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            //Manually validate the token
            ClaimsPrincipal claims = _jwtService.ValidateJwtToken(token);
            //Get the user ID claim from the token
            string tokenUserId = claims.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException(ErrorMessageHelper.InvalidNameIdentifierMessage());

            if (int.TryParse(tokenUserId, out int userId))
            {
                await _paragraphService.DeleteByIdAsync(projectId: projectId, paragraphId: paragraphId, tokenUserId: userId);
                return NoContent();
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

    //Delete a challenge for a specific project
    [HttpDelete("{projectId}/challenges/{challengeId}")]
    public async Task<IActionResult> DeleteProjectChallenge(int projectId, int challengeId)
    {
        try
        {
            //Retrieve the access token from the request
            var token = HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            //Manually validate the token
            ClaimsPrincipal claims = _jwtService.ValidateJwtToken(token);
            //Get the user ID claim from the token
            string tokenUserId = claims.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException(ErrorMessageHelper.InvalidNameIdentifierMessage());

            if (int.TryParse(tokenUserId, out int userId))
            {
                await _challengeService.DeleteByIdAsync(projectId: projectId, challengeId: challengeId, tokenUserId: userId);
                return NoContent();
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
