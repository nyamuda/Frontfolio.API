using Frontfolio.API.Models;

namespace Frontfolio.API.Services.Abstractions
{
    public interface IParagraphService
    {
        Task DeleteByIdAsync(int parentId, int paragraphId, int tokenUserId);

        Task AddIfNotExistingAsync(int parentId, List<Paragraph> incomingParagraphs);

        Task UpdateExistingAsync(int parentId, List<Paragraph> incomingParagraphs);
    }
}
