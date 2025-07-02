using Frontfolio.API.Models;

namespace Frontfolio.API.Services.Abstractions
{
    public interface IFeedbackService
    {
        Task DeleteByIdAsync(int projectId, int feedbackId, int tokenUserId);

        Task AddIfNotExistingAsync(int projectId, List<Feedback> incomingFeedback);

        Task UpdateExistingAsync(int projectId, List<Feedback> incomingFeedback);
    }
}
