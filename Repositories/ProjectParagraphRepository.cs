using Frontfolio.API.Data;
using Frontfolio.API.Models;
using Frontfolio.API.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;


public class ProjectParagraphRepository : IProjectParagraphRepository
{
    private readonly ApplicationDbContext _context;

    public ProjectParagraphRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> DeleteByIdAsync(int id)
    {
        var paragraph = await _context.Paragraphs.FirstOrDefaultAsync(p => p.Id.Equals(id));
        if (paragraph is null) return false;

        _context.Paragraphs.Remove(paragraph);
        await _context.SaveChangesAsync();
        return true;

    }


    public async Task<bool> AddIfNotExistingAsync(int projectId, List<Paragraph> incomingParagraphs)
    {
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id.Equals(projectId));

        if (project is null) return false;

        // Get a list of IDs for paragraphs that are already part of the project
        List<int> existingParagraphIds = project.Background.Select(p => p.Id).ToList();

        // Filter incoming paragraphs to exclude any that already exist
        var uniqueParagraphs = incomingParagraphs
            .Where(p => !existingParagraphIds.Contains(p.Id))
            .ToList();

        // Add the unique paragraphs to the project
        project.Background.AddRange(uniqueParagraphs);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateExistingAsync(int projectId, List<Paragraph> incomingParagraphs)
    {
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id.Equals(projectId));

        if (project is null) return false;

        // Loop through each existing paragraph and try to find a match in the incoming list
        foreach (var existingParagraph in project.Background)
        {
            var updatedParagraph = incomingParagraphs.FirstOrDefault(p => p.Id == existingParagraph.Id);

            if (updatedParagraph is not null)
            {
                // Update only if values have changed 
                existingParagraph.Title = updatedParagraph.Title;
                existingParagraph.Content = updatedParagraph.Content;
                existingParagraph.ImageUrl = updatedParagraph.ImageUrl;
                existingParagraph.ImageCaption = updatedParagraph.ImageCaption;
                existingParagraph.UpdatedAt = DateTime.UtcNow;
            }
        }
        await _context.SaveChangesAsync();

        return true;

    }
}