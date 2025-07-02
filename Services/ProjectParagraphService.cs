
using Frontfolio.API.Data;
using Frontfolio.API.Models;
using Frontfolio.API.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

public class ProjectParagraphService:IParagraphService
{

    private readonly ApplicationDbContext _context;


    public ProjectParagraphService(ApplicationDbContext context)
    {
        _context = context;
       

    }

    /// <summary>
    /// Deletes a background paragraph for a specific project.
    /// </summary>
    /// <param name="projectId">The ID of the project to which the background paragraph belongs.</param>
    /// <param name="paragraphId">The ID of the background paragraph to delete.</param>
    /// <param name="tokenUserId">The ID of the user making the request, extracted from the JWT token.</param>
    /// <exception cref="KeyNotFoundException">
    /// Thrown when the project or the specified background paragraph cannot be found.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// Thrown when the user attempting the delete is not the owner of the project.
    /// </exception>
    /// <remarks>
    /// This method ensures that only the owner of the project can delete its background paragraph. 
    /// It validates the existence of both the project and paragraph before deletion.
    /// </remarks>
    public async Task DeleteByIdAsync(int projectId, int paragraphId, int tokenUserId)
    {
        //check if project with the given ID exists
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id.Equals(projectId))
           ?? throw new KeyNotFoundException($@"Project with ID ""{projectId}"" does not exist.");

        //Check if background paragraph with the given ID and ProjectID exists
        var background = await _context.Paragraphs
            .FirstOrDefaultAsync(p => p.Id.Equals(paragraphId) && p.ProjectId.Equals(projectId))
            ?? throw new KeyNotFoundException($@"Project background paragraph with ID ""{paragraphId}"" and ProjectId ""{projectId}"" does not exist.");


        // A user is only allowed to delete their own projects.
        ProjectHelper.EnsureUserOwnsProject(tokenUserId, project);
       
        _context.Paragraphs.Remove(background);
        await _context.SaveChangesAsync();
    }
    /// <summary>
    /// Adds new background paragraphs to a project by excluding any paragraphs
    /// that already exist in the project's current background list.
    /// </summary>
    /// <param name="projectId">The ID of the project to update.</param>
    /// <param name="incomingParagraphs">The full list of incoming background paragraphs (both new and possibly existing ones).</param>
    /// <exception cref="KeyNotFoundException">Thrown if a project with the given ID is not found.</exception>
    public async Task AddIfNotExistingAsync(int projectId, List<Paragraph> incomingParagraphs)
    {
        // Retrieve the existing project with the given ID
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId)
            ?? throw new KeyNotFoundException($@"Project with ID ""{projectId}"" does not exist.");

        // Get a list of IDs for paragraphs that are already part of the project
        List<int> existingParagraphIds = project.Background.Select(p => p.Id).ToList();

        // Filter incoming paragraphs to exclude any that already exist
        var uniqueParagraphs = incomingParagraphs
            .Where(p => !existingParagraphIds.Contains(p.Id))
            .ToList();

        // Add the unique paragraphs to the project
        project.Background.AddRange(uniqueParagraphs);

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Updates existing background paragraphs for a given project by applying any changes
    /// from the incoming list of paragraphs that match by ID.
    /// </summary>
    /// <param name="projectId">The ID of the project whose paragraphs should be updated.</param>
    /// <param name="incomingParagraphs">The list of paragraphs containing updated content.</param>
    /// <exception cref="KeyNotFoundException">Thrown if the project with the specified ID is not found.</exception>
    public async Task UpdateExistingAsync(int projectId, List<Paragraph> incomingParagraphs)
    {
        // Retrieve the project including its current background paragraphs
        var project = await _context.Projects
            .Include(p => p.Background)
            .FirstOrDefaultAsync(p => p.Id == projectId)
            ?? throw new KeyNotFoundException($@"Project with ID ""{projectId}"" does not exist.");

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
    }



}
