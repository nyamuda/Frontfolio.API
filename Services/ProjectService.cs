
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
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id.Equals(id));


        if (project is null) throw new KeyNotFoundException($@"Project with ID ""{id}"" doest not exist");

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
    /// - A <see cref="PageInfo"/> object with pagination metadata and a list of the projects.
    /// </returns>
    public async Task<PageInfo<ProjectDto>> GetProjects(int page, int pageSize, int userId, ProjectSortOption? sortOption)
    {
        var query = _context.Projects.Where(p => p.UserId.Equals(userId)).AsQueryable();

        //sort the projects by the sortOption
        query = sortOption switch
        {
            ProjectSortOption.SortOrder => query.OrderByDescending(p => p.SortOrder),
            ProjectSortOption.StartDate => query.OrderByDescending(p => p.StartDate),
            ProjectSortOption.EndDate => query.OrderByDescending(p => p.EndDate),
            ProjectSortOption.DifficultyLevel => query.OrderByDescending(p =>p.DifficultyLevel),
            _ => query.OrderByDescending(p => p.CreatedAt) 
        };

        List<ProjectDto> projects = await query           
             .Skip((page - 1) * pageSize)
             .Take(pageSize)       
             .Select(p => new ProjectDto
             {
                 Id = p.Id,
                 Title = p.Title,
                 SortOrder=p.SortOrder,
                 DifficultyLevel=p.DifficultyLevel,
                 StartDate=p.StartDate,
                 EndDate=p.EndDate,
                 Summary = p.Summary,
                 Status = p.Status,
                 TechStack = p.TechStack,
                 ImageUrl = p.ImageUrl,
                 VideoUrl=p.VideoUrl,
                 LiveUrl = p.LiveUrl,
                 GitHubUrl = p.GitHubUrl,
                 CreatedAt = p.CreatedAt,
                 UpdatedAt = p.UpdatedAt
             }).ToListAsync();

        //check if there are still more projects for the user
        int totalProjects = await _context.Projects.Where(p => p.UserId.Equals(userId)).CountAsync();
        bool hasMore = totalProjects > page * pageSize;

        PageInfo<ProjectDto> pageInfo = new() { Page = page, PageSize = pageSize, HasMore = hasMore, Items=projects };

        return pageInfo;

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

        //convert start and end dates to UTC time
        project.StartDate = TimeZoneInfo.ConvertTimeToUtc(project.StartDate);
        project.EndDate = TimeZoneInfo.ConvertTimeToUtc(project.EndDate);

        //save the new project
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        return ProjectDto.MapFrom(project);
    }

   
    //Update an existing project
    public async Task UpdateProject(int userId,int projectId,UpdateProjectDto updateProjectDto)
    {
        //check if user with the given ID exist
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(userId))
            ?? throw new KeyNotFoundException($@"User with ID ""{userId}"" does not exist.");

        //get the project
        Project project = await _context.Projects.FirstOrDefaultAsync(p =>p.Id.Equals(projectId)) 
            ?? throw new KeyNotFoundException(($@"Project with ID ""{projectId}"" does not exist."));
        //update the project
        project.Status=updateProjectDto.Status;
        project.Title=updateProjectDto.Title;
        project.SortOrder=updateProjectDto.SortOrder;
        project.DifficultyLevel=updateProjectDto.DifficultyLevel;
        project.StartDate= TimeZoneInfo.ConvertTimeToUtc(updateProjectDto.StartDate); ;
        project.EndDate= TimeZoneInfo.ConvertTimeToUtc(updateProjectDto.EndDate);
        project.Summary=updateProjectDto.Summary;
        project.GitHubUrl = updateProjectDto.GitHubUrl;
        project.ImageUrl=updateProjectDto.ImageUrl;
        project.LiveUrl=updateProjectDto.LiveUrl;
        project.VideoUrl=updateProjectDto.VideoUrl; 
        project.TechStack=updateProjectDto.TechStack;
        project.Background=updateProjectDto.Background;
        project.Challenges=updateProjectDto.Challenges;
        project.Achievements=updateProjectDto.Achievements;
        project.Feedback=updateProjectDto.Feedback;
        project.UpdatedAt= DateTime.UtcNow;

        await _context.SaveChangesAsync();
        
    }

    //Delete a project
    public async Task DeleteProject(int id)
    {
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id.Equals(id)) 
            ?? throw new KeyNotFoundException($@"Project with ID ""{id}"" does not exist.");

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
    }




}

