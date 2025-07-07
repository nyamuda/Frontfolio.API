
using Frontfolio.API.Data;
using Frontfolio.API.Dtos.Auth;
using Frontfolio.API.Models;
using Frontfolio.API.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

public class ProjectService : IProjectService
{

    private readonly ApplicationDbContext _context;
    private readonly ProjectParagraphService _paragraphService;
    private readonly ChallengeService _challengeService;
    private readonly AchievementService _achievementService;
    private readonly FeedbackService _feedbackService;

    public ProjectService(ApplicationDbContext context,
        ProjectParagraphService paragraphService,
        ChallengeService challengeService,
        AchievementService achievementService,
        FeedbackService feedbackService)
    {
        _context = context;
        _paragraphService = paragraphService;
        _challengeService = challengeService;
        _achievementService = achievementService;
        _feedbackService = feedbackService;
    }

    //Get a project with a given ID 
    public async Task<ProjectDto> GetAsync(int projectId, int tokenUserId)
    {
        var project = await _context.Projects
            .Include(p => p.Background)
            .Include(p => p.Achievements)
            .Include(p => p.Challenges)
            .Include(p => p.Feedback)
            .AsSplitQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id.Equals(projectId));


        if (project is null) throw new KeyNotFoundException($@"Project with ID ""{projectId}"" doest not exist. Please check the URL or try again later.");

        //Only the owner the project is allowed to access it
        ProjectHelper.EnsureUserOwnsProject(tokenUserId, project,crudContext:CrudContext.Read);

        return ProjectDto.MapFrom(project);
    }

    /// <summary>
    /// Gets all the projects for a user with a given ID.
    /// Supports pagination by returning only a specific page of results,
    /// along with paging information indicating if more projects exist.
    /// </summary>
    /// <param name="page">The current page number (1-based).</param>
    /// <param name="pageSize">The number of projects to return per page.</param>
    /// <param name="userId">The unique identifier of the user whose projects are being retrieved.</param>
    /// <returns>
    /// A tuple containing:
    /// - A <see cref="PageInfo"/> object with pagination metadata and a list of the projects.
    /// </returns>
    public async Task<PageInfo<ProjectDto>> GetAllAsync(int page, int pageSize, int userId, ProjectSortOption sortOption,ProjectStatusFilter filterOption)
    {
        var query = _context.Projects.Where(p => p.UserId.Equals(userId)).AsQueryable();

        //filter the projects by status
        query = filterOption switch { 
            ProjectStatusFilter.Published=> query.Where(p =>p.Status.Equals(ProjectStatus.Published)),
            ProjectStatusFilter.Draft =>query.Where(p => p.Status.Equals(ProjectStatus.Draft)),
            _=>query
        };

        //sort the projects by the provided sortOption
        query = sortOption switch
        {
            ProjectSortOption.Title => query.OrderByDescending(p => p.Title),  
            ProjectSortOption.DifficultyLevel => query.OrderByDescending(p => p.DifficultyLevel),
            ProjectSortOption.CreatedAt => query.OrderByDescending(p => p.CreatedAt),
            _ => query.OrderBy(p => p.SortOrder) //default sort option is `SortOrder`
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
                 UserId=p.UserId,
                 TechStack = p.TechStack,
                 ImageUrl = p.ImageUrl,
                 VideoUrl = p.VideoUrl,
                 LiveUrl = p.LiveUrl,
                 GitHubUrl = p.GitHubUrl,
                 CreatedAt = p.CreatedAt,
                 UpdatedAt = p.UpdatedAt
             }).ToListAsync();

        //check if there are still more projects for the user
        int totalProjects = await query.CountAsync();
        bool hasMore = totalProjects > page * pageSize;

        PageInfo<ProjectDto> pageInfo = new() { Page = page, PageSize = pageSize, HasMore = hasMore, Items = projects };

        return pageInfo;

    }

    //Add a new project
    public async Task<ProjectDto> CreateAsync(int userId, AddProjectDto addProjectDto)
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
    public async Task UpdateAsync(int projectId, int tokenUserId, UpdateProjectDto updateProjectDto)
    {
        //get the existing project
        Project existingProject = await _context.Projects.FirstOrDefaultAsync(p => p.Id.Equals(projectId))
            ?? throw new KeyNotFoundException(($@"Project with ID ""{projectId}"" does not exist."));

        //Only the owner the project is allowed to update it
        ProjectHelper.EnsureUserOwnsProject(tokenUserId, existingProject, crudContext: CrudContext.Update);


        //map UpdateProjectDto to Project
        Project updatedProject = UpdateProjectDto.MapTo(updateProjectDto);

       

        // Update all properties
        existingProject.Title = updatedProject.Title;
        existingProject.Status = updatedProject.Status;
        existingProject.SortOrder = updatedProject.SortOrder;
        existingProject.DifficultyLevel = updatedProject.DifficultyLevel;
        existingProject.StartDate = updatedProject.StartDate;
        existingProject.EndDate = updatedProject.EndDate;
        existingProject.Summary = updatedProject.Summary;
        existingProject.GitHubUrl = updatedProject.GitHubUrl;
        existingProject.ImageUrl = updatedProject.ImageUrl;
        existingProject.VideoUrl = updatedProject.VideoUrl;
        existingProject.LiveUrl = updatedProject.LiveUrl;
        existingProject.TechStack = updatedProject.TechStack;
        existingProject.UpdatedAt = updatedProject.UpdatedAt;

        await _context.SaveChangesAsync();

        //STEP 1. Update the nested background paragraphs
        //The updated background paragraph list contains the updated paragraphs as well as some new ones
        //add the new paragraphs
        await _paragraphService.AddIfNotExistingAsync(existingProject.Id,updatedProject.Background);
        //update existing ones
        await _paragraphService.UpdateExistingAsync(existingProject.Id, updatedProject.Background);

        //STEP 2. Update the nested challenges
        //The updated challenge list contains the updated challenges as well as some new ones
        //add the new challenges
        await _challengeService.AddIfNotExistingAsync(existingProject.Id, updatedProject.Challenges);
        //update existing ones
        await _challengeService.UpdateExistingAsync(existingProject.Id, updatedProject.Challenges);

        //STEP 3. Update the nested achievements
        //The updated achievement list contains the updated achievements as well as some new ones
        //add the new achievements
        await _achievementService.AddIfNotExistingAsync(existingProject.Id, updatedProject.Achievements);
        //update existing ones
        await _achievementService.UpdateExistingAsync(existingProject.Id, updatedProject.Achievements);

        //STEP 4. Update the nested feedback items
        //The updated feedback list contains the updated feedback items as well as new ones
        //add the new feedback items
        await _feedbackService.AddIfNotExistingAsync(existingProject.Id, updatedProject.Feedback);
        //update existing ones
        await _feedbackService.UpdateExistingAsync(existingProject.Id, updatedProject.Feedback);




    }

    //Delete a project
    public async Task DeleteAsync(int projectId,int tokenUserId)
    {
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id.Equals(projectId))
            ?? throw new KeyNotFoundException($@"Project with ID ""{projectId}"" does not exist.");

        //Only the owner the project is allowed to delete it
        ProjectHelper.EnsureUserOwnsProject(tokenUserId, project, crudContext: CrudContext.Delete);

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
    }

    //Publish a project
    public async Task PublishAsync(int projectId,int tokenUserId)
    {
        //get the project to be published
        var project = await _context.Projects.FirstOrDefaultAsync(p =>p.Id.Equals(projectId))
            ?? throw new KeyNotFoundException($@"Project with ID ""{projectId}"" does not exist.");

        //Only the owner the project is allowed to publish it
        ProjectHelper.EnsureUserOwnsProject(tokenUserId, project, crudContext: CrudContext.Update);

        //check if project is not already published
        if(project.Status.Equals(ProjectStatus.Published))
            throw new InvalidOperationException("Project is already published")

    }


}

