using Frontfolio.API.Models;

namespace Frontfolio.API.Repositories.Abstractions
{
    public interface IFeedbackRepository
    {
        Task<bool> DeleteByIdAsync(int id);

        Task<bool> AddIfNotExistingAsync(int projectId, List<Feedback> incomingFeedback);

        Task<bool> UpdateExistingAsync(int projectId, List<Feedback> incomingFeedback);
    }
}
