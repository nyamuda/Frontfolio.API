
using Frontfolio.API.Data;
using Frontfolio.API.Dtos.Auth;
using Frontfolio.API.Models;
using Frontfolio.API.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

public class ProjectService:IProjectService
{

    private readonly ApplicationDbContext _context;
    private readonly ProjectParagraphService _paragraphService;

    public ProjectService(ApplicationDbContext context, ProjectParagraphService paragraphService)
    {
        _context = context;
        _paragraphService = paragraphService;
    }

    //Get a project with a given ID 
    public async Task<ProjectDto> Get(int id, int tokenUserId)
    {
        var project = await _context.Projects
            .Include(p => p.Background)
            .AsSplitQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id.Equals(id));


        if (project is null) throw new KeyNotFoundException($@"Project with ID ""{id}"" doest not exist. Please check the URL or try again later.");

        // A user is only allowed to access their own project
        ProjectHelper.EnsureUserOwnsProject(tokenUserId, project);

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
            ProjectSortOption.DifficultyLevel => query.OrderByDescending(p => p.DifficultyLevel),
            _ => query.OrderByDescending(p => p.CreatedAt)
        };

        List<ProjectDto> projects = await query
             .Skip((page - 1) * pageSize)
             .Take(pageSize)
             .Select(p => new ProjectDto
             {
                 Id = p.Id,
                 Title = p.Title,
                 SortOrder = p.SortOrder,
                 DifficultyLevel = p.DifficultyLevel,
                 StartDate = p.StartDate,
                 EndDate = p.EndDate,
                 Summary = p.Summary,
                 Status = p.Status,
                 TechStack = p.TechStack,
                 ImageUrl = p.ImageUrl,
                 VideoUrl = p.VideoUrl,
                 LiveUrl = p.LiveUrl,
                 GitHubUrl = p.GitHubUrl,
                 CreatedAt = p.CreatedAt,
                 UpdatedAt = p.UpdatedAt
             }).ToListAsync();

        //check if there are still more projects for the user
        int totalProjects = await _context.Projects.Where(p => p.UserId.Equals(userId)).CountAsync();
        bool hasMore = totalProjects > page * pageSize;

        PageInfo<ProjectDto> pageInfo = new() { Page = page, PageSize = pageSize, HasMore = hasMore, Items = projects };

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
    public async Task UpdateProject(int userId, int projectId, UpdateProjectDto updateProjectDto)
    {
        //check if user with the given ID exist
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(userId))
            ?? throw new KeyNotFoundException($@"User with ID ""{userId}"" does not exist.");

        //map UpdateProjectDto to Project
        Project project = UpdateProjectDto.MapTo(updateProjectDto);

        //get the project
        Project existingProject = await _context.Projects.FirstOrDefaultAsync(p => p.Id.Equals(projectId))
            ?? throw new KeyNotFoundException(($@"Project with ID ""{projectId}"" does not exist."));

        // Update all properties
        existingProject.Title = project.Title;
        existingProject.Status = project.Status;
        existingProject.SortOrder = project.SortOrder;
        existingProject.DifficultyLevel = project.DifficultyLevel;
        existingProject.StartDate = TimeZoneInfo.ConvertTimeToUtc(project.StartDate);
        existingProject.EndDate = TimeZoneInfo.ConvertTimeToUtc(project.EndDate);
        existingProject.Summary = project.Summary;
        existingProject.GitHubUrl = project.GitHubUrl;
        existingProject.ImageUrl = project.ImageUrl;
        existingProject.VideoUrl = project.VideoUrl;
        existingProject.LiveUrl = project.LiveUrl;
        existingProject.TechStack = project.TechStack;
        existingProject.UpdatedAt = DateTime.UtcNow;

        await _paragraphService.AddUniqueBackgroundParagraphsAsync(existingProject.Id, project.Background);
        await _paragraphService.UpdateExistingBackgroundParagraphsAsync(existingProject.Id, project.Background);

        //get the project
        //get an list of Ids of existing project background paragraphs
        //loop the new paragraphs and check if the Id of each existing  in the list of Ids
        //if so,remove that new paragraph from the list of new paragraphs
        //finally, save the remaining new paragraphs


        // Check if the existing project has background paragraphs
        // and the updated project has removed all of them.
        // If so, delete all the existing background paragraphs from the database.
        //if (existingProject.Background.Count > 0 && project.Background.Count == 0)
        //{
        //    _context.Paragraphs.RemoveRange(existingProject.Background);
        //}

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

