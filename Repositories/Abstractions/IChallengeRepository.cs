using Frontfolio.API.Models;

namespace Frontfolio.API.Repositories.Abstractions
{
    public interface IChallengeRepository
    {
        Task<bool> DeleteByIdAsync(int id);

        Task<bool> AddIfNotExistingAsync(int projectId, List<Challenge> incomingChallenges);

        Task<bool> UpdateExistingAsync(int projectId, List<Challenge> incomingChallenges);
    }
}
