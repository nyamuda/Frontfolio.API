using Frontfolio.API.Data;
using Frontfolio.API.Models;
using Frontfolio.API.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Frontfolio.API.Repositories
{
    public class FeedbackRepository:IFeedbackRepository
    {
        private readonly ApplicationDbContext _context;

        public FeedbackRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            var feedback = await _context.Feedback.FirstOrDefaultAsync(p => p.Id.Equals(id));
            if (feedback is null) return false;

            _context.Feedback.Remove(feedback);
            await _context.SaveChangesAsync();
            return true;

        }
        public async Task<bool> AddIfNotExistingAsync(int projectId, List<Feedback> incomingParagraphs)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id.Equals(projectId));

            if (project is null) return false;

            // Get a list of IDs for paragraphs that are already part of the project
            List<int> existingParagraphIds = project.Background.Select(p => p.Id).ToList();

            // Filter incoming feedback to exclude any that already exist
            var uniqueFeedback = incomingParagraphs
                .Where(p => !existingParagraphIds.Contains(p.Id))
                .ToList();

            // Add the unique feedback to the project
            project.Feedback.AddRange(uniqueFeedback);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateExistingAsync(int projectId, List<Feedback> incomingFeedback)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id.Equals(projectId));

            if (project is null) return false;

            // Loop through each existing feedback and try to find a match in the incoming list
            foreach (var existingFeedback in project.Feedback)
            {
                var updatedFeedback = incomingFeedback.FirstOrDefault(p => p.Id == existingFeedback.Id);

                if (updatedFeedback is not null)
                {
                    // Update only if values have changed 
                    existingFeedback.AuthorName = updatedFeedback.AuthorName;
                    existingFeedback.AuthorRole = updatedFeedback.AuthorRole;
                    existingFeedback.Comment = updatedFeedback.Comment;
                    existingFeedback.SubmittedAt = updatedFeedback.SubmittedAt;
                    
                }
            }
            await _context.SaveChangesAsync();

            return true;

        }


    }
}
