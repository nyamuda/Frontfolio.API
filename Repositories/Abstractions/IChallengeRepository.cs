using Frontfolio.API.Models;

namespace Frontfolio.API.Repositories.Abstractions
{
    public interface IChallengeRepository
    {
        Task<ParagraphDto> DeleteByIdAsync(int id, int projectId);

        Task AddIfNotExistingAsync(int projectId, List<Challenge> incomingChallenges);

        Task UpdateExistingAsync(int projectId, List<Challenge> incomingChallenges);
    }
}
