using Frontfolio.API.Data;
using Frontfolio.API.Models;
using Frontfolio.API.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

    public class FeedbackService:IFeedbackService
    {
        private readonly ApplicationDbContext _context;

        public FeedbackService(ApplicationDbContext context)
        {
            _context = context;
        }

    /// <summary>
    /// Deletes feedback for a specific project.
    /// </summary>
    /// <param name="projectId">The ID of the project to which the feedback belongs.</param>
    /// <param name="feedbackId">The ID of the feedback to delete.</param>
    /// <param name="tokenUserId">The ID of the user making the request, extracted from the JWT token.</param>
    /// <exception cref="KeyNotFoundException">
    /// Thrown when the project or the specified feedback cannot be found.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// Thrown when the user attempting the delete is not the owner of the project.
    /// </exception>
    /// <remarks>
    /// This method ensures that only the owner of the project can delete its feedback. 
    /// It validates the existence of both the project and feedback before deletion.
    /// </remarks>
    public async Task DeleteByIdAsync(int projectId, int feedbackId, int tokenUserId)
    {
        //check if project with the given ID exists
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id.Equals(projectId))
           ?? throw new KeyNotFoundException($@"Project with ID ""{projectId}"" does not exist.");

        //Check if feedback with the given ID and ProjectID exists
        var feedback = await _context.Feedback
            .FirstOrDefaultAsync(p => p.Id.Equals(feedbackId) && p.ProjectId.Equals(projectId))
            ?? throw new KeyNotFoundException($@"Project feedback with ID ""{feedbackId}"" and ProjectId ""{projectId}"" does not exist.");


        // A user is only allowed to delete their own project feedback.
        ProjectHelper.EnsureUserOwnsProject(tokenUserId, project);

        _context.Feedback.Remove(feedback);
        await _context.SaveChangesAsync();
    }


    /// <summary>
    /// Adds new feedback to a project by excluding any feedback
    /// that already exist in the project's current feedback list.
    /// </summary>
    /// <param name="projectId">The ID of the project to update.</param>
    /// <param name="incomingFeedback">The full list of incoming feedback (both new and possibly existing one).</param>
    /// <exception cref="KeyNotFoundException">Thrown if a project with the given ID is not found.</exception>
    public async Task AddIfNotExistingAsync(int projectId, List<Feedback> incomingFeedback)
    {
        // Retrieve the project including its current feedback
        var project = await _context.Projects
            .Include(p=>p.Feedback)
            .FirstOrDefaultAsync(p => p.Id == projectId)
            ?? throw new KeyNotFoundException($@"Project with ID ""{projectId}"" does not exist.");

        // Get a list of IDs for feedback that is already part of the project
        List<int> existingFeedbackIds = project.Feedback.Select(f => f.Id).ToList();

        // Filter incoming feedback to exclude any that already exist
        var uniqueFeedback = incomingFeedback
            .Where(f => !existingFeedbackIds.Contains(f.Id))
            .ToList();

        // Add the unique feedback to the project
        project.Feedback.AddRange(uniqueFeedback);

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Updates existing feedback for a given project by applying any changes
    /// from the incoming list of feedback that match by ID.
    /// </summary>
    /// <param name="projectId">The ID of the project whose feedback should be updated.</param>
    /// <param name="incomingFeedback">The list of feedback containing updated content.</param>
    /// <exception cref="KeyNotFoundException">Thrown if the project with the specified ID is not found.</exception>
    public async Task UpdateExistingAsync(int projectId, List<Feedback> incomingFeedback)
    {
        // Retrieve the project including its current feedback
        var project = await _context.Projects
            .Include(p => p.Feedback)
            .FirstOrDefaultAsync(p => p.Id == projectId)
            ?? throw new KeyNotFoundException($@"Project with ID ""{projectId}"" does not exist.");

        // Loop through each existing feedback and try to find a match in the incoming list
        foreach (var existingFeedback in project.Feedback)
        {
            var updatedFeedback = incomingFeedback.FirstOrDefault(f => f.Id == existingFeedback.Id);

            if (updatedFeedback is not null)
            {
                // Update only if values have changed 
                existingFeedback.AuthorName = updatedFeedback.AuthorName;
                existingFeedback.AuthorRole = updatedFeedback.AuthorRole;
                existingFeedback.Comment = updatedFeedback.Comment;
                existingFeedback.SubmittedAt = updatedFeedback.SubmittedAt;
                existingFeedback.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync();
    }



}

