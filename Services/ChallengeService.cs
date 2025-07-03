using Frontfolio.API.Data;
using Frontfolio.API.Services.Abstractions;
using Microsoft.EntityFrameworkCore;


public class ChallengeService : IChallengeService
{

    private readonly ApplicationDbContext _context;

    public ChallengeService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Deletes a challenge for a specific project.
    /// </summary>
    /// <param name="projectId">The ID of the project to which the challenge belongs.</param>
    /// <param name="challengeId">The ID of the challenge to delete.</param>
    /// <param name="tokenUserId">The ID of the user making the request, extracted from the JWT token.</param>
    /// <exception cref="KeyNotFoundException">
    /// Thrown when the project or the specified challenge cannot be found.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// Thrown when the user attempting the delete is not the owner of the project.
    /// </exception>
    /// <remarks>
    /// This method ensures that only the owner of the project can delete its challenge. 
    /// It validates the existence of both the project and challenge before deletion.
    /// </remarks>
    public async Task DeleteByIdAsync(int projectId, int challengeId, int tokenUserId)
    {
        //check if project with the given ID exists
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id.Equals(projectId))
           ?? throw new KeyNotFoundException($@"Project with ID ""{projectId}"" does not exist.");

        //Check if challenge with the given ID and ProjectID exists
        var challenge = await _context.Challenges
            .FirstOrDefaultAsync(c => c.Id.Equals(challengeId) && c.ProjectId.Equals(projectId))
            ?? throw new KeyNotFoundException($@"Project challenge with ID ""{challengeId}"" and ProjectId ""{projectId}"" does not exist.");


        //Only the owner the project is allowed to delete its challenge
        ProjectHelper.EnsureUserOwnsProject(tokenUserId, project, crudContext: CrudContext.Delete);

        _context.Challenges.Remove(challenge);
        await _context.SaveChangesAsync();
    }


    /// <summary>
    /// Adds new challenges to a project by excluding any challenges
    /// that already exist in the project's current challenges list.
    /// </summary>
    /// <param name="projectId">The ID of the project to update.</param>
    /// <param name="incomingChallenges">The full list of incoming challenges (both new and possibly existing one).</param>
    /// <exception cref="KeyNotFoundException">Thrown if a project with the given ID is not found.</exception>
    public async Task AddIfNotExistingAsync(int projectId, List<Challenge> incomingChallenges)
    {
        // Retrieve the project including its current challenges
        var project = await _context.Projects
            .Include(p => p.Challenges)
            .FirstOrDefaultAsync(p => p.Id == projectId)
            ?? throw new KeyNotFoundException($@"Project with ID ""{projectId}"" does not exist.");

        // Get a list of IDs for challenges that are already part of the project
        List<int> existingChallengeIds = project.Challenges.Select(c => c.Id).ToList();

        // Filter incoming challenges to exclude any that already exist
        var uniqueChallenges = incomingChallenges
            .Where(c => !existingChallengeIds.Contains(c.Id))
            .ToList();

        // Add the unique challenges to the project
        project.Challenges.AddRange(uniqueChallenges);

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Updates existing challenges for a given project by applying any changes
    /// from the incoming list of challenges that match by ID.
    /// </summary>
    /// <param name="projectId">The ID of the project whose challenges should be updated.</param>
    /// <param name="incomingChallenges">The list of challenges containing updated content.</param>
    /// <exception cref="KeyNotFoundException">Thrown if the project with the specified ID is not found.</exception>
    public async Task UpdateExistingAsync(int projectId, List<Challenge> incomingChallenges)
    {
        // Retrieve the project including its current challenges
        var project = await _context.Projects
            .Include(p => p.Challenges)
            .FirstOrDefaultAsync(p => p.Id == projectId)
            ?? throw new KeyNotFoundException($@"Project with ID ""{projectId}"" does not exist.");

        // Loop through each existing challenge and try to find a match in the incoming list
        foreach (var existingChallenge in project.Challenges)
        {
            var updatedChallenge = incomingChallenges.FirstOrDefault(c => c.Id == existingChallenge.Id);

            if (updatedChallenge is not null)
            {

                existingChallenge.Title = updatedChallenge.Title;
                existingChallenge.Problem = updatedChallenge.Problem;
                existingChallenge.Solution = updatedChallenge.Solution;
                existingChallenge.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync();
    }

}

