using Frontfolio.API.Models;

namespace Frontfolio.API.Repositories.Abstractions
{
    public interface IAchievementRepository
    {
        Task<ParagraphDto> DeleteByIdAsync(int id, int projectId);

        Task AddIfNotExistingAsync(int projectId, List<Achievement> incomingAchievements);

        Task UpdateExistingAsync(int projectId, List<Achievement> incomingAchievements);
    }
}
