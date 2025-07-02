using Frontfolio.API.Models;

namespace Frontfolio.API.Services.Abstractions
{
    public interface IChallengeService
    {
        Task DeleteByIdAsync(int projectId, int challengeId, int tokenUserId);

        Task AddIfNotExistingAsync(int projectId, List<Challenge> incomingChallenges);

        Task UpdateExistingAsync(int projectId, List<Challenge> incomingChallenges);
    }
}
