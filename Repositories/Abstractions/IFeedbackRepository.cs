using Frontfolio.API.Models;

namespace Frontfolio.API.Repositories.Abstractions
{
    public interface IFeedbackRepository
    {
        Task<ParagraphDto> DeleteByIdAsync(int id, int projectId);

        Task AddIfNotExistingAsync(int projectId, List<Feedback> incomingFeedback);

        Task UpdateExistingAsync(int projectId, List<Feedback> incomingFeedback);
    }
}
