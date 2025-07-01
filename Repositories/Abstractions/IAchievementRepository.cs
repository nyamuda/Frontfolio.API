using Frontfolio.API.Models;

namespace Frontfolio.API.Repositories.Abstractions
{
    public interface IAchievementRepository
    {
        Task<bool> DeleteByIdAsync(int id);

        Task<bool> AddIfNotExistingAsync(int projectId, List<Achievement> incomingAchievements);

        Task<bool> UpdateExistingAsync(int projectId, List<Achievement> incomingAchievements);
    }
}
