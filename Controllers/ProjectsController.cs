﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


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
            if (project.UserId != int.Parse(tokenUserId))
                return Forbid("You don't have permission to view this project.");

            return Ok(project);


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
    [Authorize]
    public async Task<IActionResult> GetProjects(int page=1,int pageSize=5)
    {
        try
        {
            //First, extract the user's access token from the Authorization header
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            //Manually validate the token and then grap the User ID from the token claims
            ClaimsPrincipal claims = _jwtService.ValidateJwtToken(token);
            string tokenUserId = claims.FindFirstValue(ClaimTypes.NameIdentifier) ??
                throw new UnauthorizedAccessException("Access denied. Token does not contain a valid user ID claim.");


            //Get all the project for a user with the given ID
            var projects = await _projectService.GetProjects(page:page,pageSize:pageSize,userId:int.Parse(tokenUserId));
        

            return Ok(projects);


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
    [Authorize]
    public async Task<IActionResult> Post(AddProjectDto addProjectDto)
    {
        try
        {
            //First, extract the user's access token from the Authorization header
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            //Manually validate the token and then grap the User ID from the token claims
            ClaimsPrincipal claims = _jwtService.ValidateJwtToken(token);
            string tokenUserId = claims.FindFirstValue(ClaimTypes.NameIdentifier) ??
                throw new UnauthorizedAccessException("Access denied. Token does not contain a valid user ID claim.");

            //Add the new project
            var project = await _projectService.AddProject(userId:int.Parse(tokenUserId),addProjectDto);

            return CreatedAtAction(nameof(Get), new { id = project.Id }, project);

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
