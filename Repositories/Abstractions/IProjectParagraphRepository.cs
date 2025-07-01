using Frontfolio.API.Models;

namespace Frontfolio.API.Repositories.Abstractions
{
    public interface IProjectParagraphRepository
    {

        Task<ParagraphDto> DeleteByIdAsync(int id,int projectId);

        Task AddIfNotExistingAsync(int projectId, List<Paragraph> incomingParagraphs);

        Task UpdateExistingAsync(int projectId, List<Paragraph> incomingParagraphs);


    }
}
