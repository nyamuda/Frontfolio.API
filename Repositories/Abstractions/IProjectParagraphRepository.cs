using Frontfolio.API.Models;

namespace Frontfolio.API.Repositories.Abstractions
{
    public interface IProjectParagraphRepository
    {

        Task<bool> DeleteByIdAsync(int id);

        Task<bool> AddIfNotExistingAsync(int projectId, List<Paragraph> incomingParagraphs);

        Task<bool> UpdateExistingAsync(int projectId, List<Paragraph> incomingParagraphs);


    }
}
