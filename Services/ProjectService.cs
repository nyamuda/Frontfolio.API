
using Frontfolio.API.Data;
using Frontfolio.API.Dtos.Auth;
using Frontfolio.API.Models;
using Microsoft.EntityFrameworkCore;

public class ProjectService
{

    private readonly ApplicationDbContext _context;

    public ProjectService(ApplicationDbContext context)
    {
        _context = context;
    }

    //Get a project with a given ID 
    public async Task<ProjectDto> GetProjectById(int id)
    {
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id.Equals(id));

        if (project is null) throw new KeyNotFoundException($"Project with ID {id} doest not exist");

        return ProjectDto.MapFrom(project);
    }

    /// <summary>
    /// Gets all the projects for a user with a given ID.
    /// Supports pagination by returning only a specific page of results,
    /// along with paging information indicating if more projects exist.
    /// </summary>
    /// <param name="page">The current page number (1-based).</param>
    /// <param name="pageSize">The number of projects to return per page.</param>
    /// <param name="UserId">The unique identifier of the user whose projects are being retrieved.</param>
    /// <returns>
    /// A tuple containing:
    /// - A list of <see cref="ProjectDto"/> objects for the specified page.
    /// - A <see cref="PageInfo"/> object with pagination metadata.
    /// </returns>
    public async Task<(List<ProjectDto> projects, PageInfo pageInfo)> GetProjects(int page, int pageSize, int userId)
    {

        List<ProjectDto> projects = await _context.Projects
             .Where(p => p.UserId.Equals(userId))
             .Skip((page - 1) * pageSize)
             .Take(pageSize)
             .OrderByDescending(p => p.CreatedAt)
             .Select(p => new ProjectDto
             {
                 Id = p.Id,
                 Title = p.Title,
                 Summary = p.Summary,
                 Status = p.Status,
                 TechStack = p.TechStack,
                 ImageUrl = p.ImageUrl,
                 LiveUrl = p.LiveUrl,
                 GitHubUrl = p.GitHubUrl,
                 CreatedAt = p.CreatedAt,
                 UpdatedAt = p.UpdatedAt
             }).ToListAsync();

        //check if there are still more projects for the user
        int totalProjects = await _context.Projects.Where(p => p.UserId.Equals(userId)).CountAsync();
        bool hasMore = totalProjects > page * pageSize;

        PageInfo pageInfo = new() { Page = page, PageSize = pageSize, HasMore = hasMore };

        return (projects, pageInfo);

    }

    //Add a new project
    public async Task<ProjectDto> AddProject(int userId, AddProjectDto addProjectDto)
    {
        //check if user with the given ID exist
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(userId));
        if (user is null) throw new KeyNotFoundException($@"User with ID ""{userId}"" does not exist.");

        //Map AddProjectDto to Project so that we can save the project to the database
        Project project = AddProjectDto.MapTo(addProjectDto);
        //add the user ID
        project.UserId = userId;

        //save the new project
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        return ProjectDto.MapFrom(project);
    }

   
    //Update an existing project
    public async Task UpdateProject(int userId,int projectId,UpdateProjectDto updateProjectDto)
    {
        //check if user with the given ID exist
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(userId));
        if (user is null) throw new KeyNotFoundException($@"User with ID ""{userId}"" does not exist.");

        //check if project with the given ID exist
        var oldProject = await _context.Projects.FirstOrDefaultAsync(p => p.Id.Equals(projectId));
        if (oldProject is null) throw new KeyNotFoundException($@"Project with ID ""{projectId}"" does not exist.");

        //Map UpdateProjectDto to Project
        Project updatedProject = UpdateProjectDto.MapTo(updateProjectDto);
        
        //add the userId and projectId fields
        updatedProject.Id = projectId;
        updatedProject.UserId = userId;

        //update project
        _context.Projects.Update(updatedProject);
        await _context.SaveChangesAsync();
        
    }




}

