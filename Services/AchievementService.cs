using Frontfolio.API.Data;
using Frontfolio.API.Services.Abstractions;
using Microsoft.EntityFrameworkCore;


public class AchievementService : IAchievementService
{
    private readonly ApplicationDbContext _context;
    public AchievementService(ApplicationDbContext context)
    {
        _context = context;
    }


    /// <summary>
    /// Deletes an achievement for a specific project.
    /// </summary>
    /// <param name="projectId">The ID of the project to which the achievement belongs.</param>
    /// <param name="achievementId">The ID of the achievement to delete.</param>
    /// <param name="tokenUserId">The ID of the user making the request, extracted from the JWT token.</param>
    /// <exception cref="KeyNotFoundException">
    /// Thrown when the project or the specified achievement cannot be found.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// Thrown when the user attempting the delete is not the owner of the project.
    /// </exception>
    /// <remarks>
    /// This method ensures that only the owner of the project can delete its achievement. 
    /// It validates the existence of both the project and achievement before deletion.
    /// </remarks>
    public async Task DeleteByIdAsync(int projectId, int achievementId, int tokenUserId)
    {
        //check if project with the given ID exists
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id.Equals(projectId))
           ?? throw new KeyNotFoundException($@"Project with ID ""{projectId}"" does not exist.");

        //Check if the achievement with the given ID and ProjectID exists
        var achievement = await _context.Achievements
            .FirstOrDefaultAsync(c => c.Id.Equals(achievementId) && c.ProjectId.Equals(projectId))
            ?? throw new KeyNotFoundException($@"Project achievement with ID ""{achievementId}"" and ProjectId ""{projectId}"" does not exist.");


        //Only the owner the project is allowed to delete its achievement
        ProjectHelper.EnsureUserOwnsProject(tokenUserId, project, crudContext: CrudContext.Delete);

        _context.Achievements.Remove(achievement);
        await _context.SaveChangesAsync();
    }


    /// <summary>
    /// Adds new achievements to a project by excluding any achievements
    /// that already exist in the project's current achievements list.
    /// </summary>
    /// <param name="projectId">The ID of the project to update.</param>
    /// <param name="incomingAchievements">The full list of incoming achievements (both new and possibly existing one).</param>
    /// <exception cref="KeyNotFoundException">Thrown if a project with the given ID is not found.</exception>
    public async Task AddIfNotExistingAsync(int projectId, List<Achievement> incomingAchievements)
    {
        // Retrieve the project including its current achievements
        var project = await _context.Projects
            .Include(p => p.Achievements)
            .FirstOrDefaultAsync(p => p.Id == projectId)
            ?? throw new KeyNotFoundException($@"Project with ID ""{projectId}"" does not exist.");

        // Get a list of IDs for achievements that are already part of the project
        List<int> existingAchievementIds = project.Achievements.Select(c => c.Id).ToList();

        // Filter incoming achievements to exclude any that already exist
        var uniqueAchievements = incomingAchievements
            .Where(c => !existingAchievementIds.Contains(c.Id))
            .ToList();

        // Add the unique achievements to the project
        project.Achievements.AddRange(uniqueAchievements);

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Updates existing achievements for a given project by applying any changes
    /// from the incoming list of achievements that match by ID.
    /// </summary>
    /// <param name="projectId">The ID of the project whose achievements should be updated.</param>
    /// <param name="incomingAchievements">The list of achievements containing updated content.</param>
    /// <exception cref="KeyNotFoundException">Thrown if the project with the specified ID is not found.</exception>
    public async Task UpdateExistingAsync(int projectId, List<Achievement> incomingAchievements)
    {
        // Retrieve the project including its current achievements
        var project = await _context.Projects
            .Include(p => p.Achievements)
            .FirstOrDefaultAsync(p => p.Id == projectId)
            ?? throw new KeyNotFoundException($@"Project with ID ""{projectId}"" does not exist.");

        // Loop through each existing achievement and try to find a match in the incoming list
        foreach (var existingAchievement in project.Achievements)
        {
            var updatedAchievement = incomingAchievements.FirstOrDefault(c => c.Id == existingAchievement.Id);

            if (updatedAchievement is not null)
            {

                existingAchievement.Title = updatedAchievement.Title;
                existingAchievement.Description = updatedAchievement.Description;
                existingAchievement.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync();
    }
}

