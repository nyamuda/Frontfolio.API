using Frontfolio.API.Models;

namespace Frontfolio.API.Services.Abstractions
{
    public interface IAchievementService
    {
        Task DeleteByIdAsync(int projectId, int achievementId, int tokenUserId);

        Task AddIfNotExistingAsync(int projectId, List<Achievement> incomingAchievements);

        Task UpdateExistingAsync(int projectId, List<Achievement> incomingAchievements);
    }
}
