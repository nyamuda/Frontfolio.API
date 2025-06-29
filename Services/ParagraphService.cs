
using Frontfolio.API.Data;
using Frontfolio.API.Models;
using Microsoft.EntityFrameworkCore;

public class ParagraphService
{

    private readonly ApplicationDbContext _context;


    public ParagraphService(ApplicationDbContext context)
    {
        _context = context;
       

    }

    //Add a background paragraph for a project with a given ID
    public async Task<ParagraphDto> AddProjectBackgroundParagraph(int projectId,int tokenUserId, AddParagraphDto paragraphDto)
    {
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id.Equals(projectId))
            ?? throw new KeyNotFoundException($@"Project with ID ""{projectId}"" does not exist.");


        // Compare the token User ID with the User ID of the project the background paragraph is about to be added to
        // A user is only allowed to add background paragraphs to their own projects.
        // If the user ID from the token does not match the project owner's ID, deny access.
        if (!project.UserId.Equals(tokenUserId))
            throw new UnauthorizedAccessException("You don't have permission to add the background to this project.");

        Paragraph paragraph = new()
        {
            Title = paragraphDto.Title,
            Content = paragraphDto.Content,
            ImageUrl = paragraphDto.ImageUrl,
            ImageCaption = paragraphDto.ImageCaption,
            ParagraphType = ParagraphType.ProjectBackground,
            ProjectId = projectId,
        };
        await _context.Paragraphs.AddAsync(paragraph);

        return ParagraphDto.MapFrom(paragraph);

    }

    //Get all the background paragraphs for a project with a given ID
    public async Task<List<ParagraphDto>> GetProjectBackgroundParagraphs(int projectId,int userId)
    {
        //check if a project with the given ID and User ID exists
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id.Equals(projectId) && p.UserId.Equals(userId)) ??
            throw new KeyNotFoundException($@"Project with ID ""{projectId}"" and UserId ""{userId}"" does not exist.");
            
        return await _context.Paragraphs
           .Where(p => p.ProjectId.Equals(project.Id))
           .OrderByDescending(p => p.CreatedAt)
           .Select(p => new ParagraphDto
           {
               Title = p.Title,
               Content = p.Content,
               ImageUrl = p.ImageUrl,
               ImageCaption = p.ImageCaption,
               ParagraphType = p.ParagraphType,
               ProjectId = p.ProjectId
           }).ToListAsync();
    }

    /// <summary>
    /// Updates a background paragraph for a specific project.
    /// </summary>
    /// <param name="projectId">The ID of the project to which the background paragraph belongs.</param>
    /// <param name="paragraphId">The ID of the background paragraph to update.</param>
    /// <param name="tokenUserId">The ID of the user making the request, extracted from the JWT token.</param>
    /// <param name="paragraphDto">The updated paragraph data.</param>
    /// <exception cref="KeyNotFoundException">
    /// Thrown when the project or the specified background paragraph cannot be found.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// Thrown when the user attempting the update is not the owner of the project.
    /// </exception>
    /// <remarks>
    /// This method ensures that only the owner of the project can update its background paragraph. 
    /// It validates the existence of both the project and paragraph before applying the changes.
    /// </remarks>
    public async Task UpdateProjectBackgroundParagraph(int projectId, int paragraphId, int tokenUserId, UpdateParagraphDto paragraphDto)
    {
        //check if project with the given ID exists
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id.Equals(projectId))
           ?? throw new KeyNotFoundException($@"Project with ID ""{projectId}"" does not exist.");

        //Check if background paragraph with the given ID and ProjectID exists
        var background = await _context.Paragraphs
            .FirstOrDefaultAsync(p => p.Id.Equals(paragraphId) && p.ProjectId.Equals(projectId))
            ?? throw new KeyNotFoundException($@"Project background paragraph with ID ""{paragraphId}"" and ProjectId ""{projectId}"" does not exist.");

        // Compare the token User ID with the User ID of the project whose background paragraph is about to be updated
        // A user is only allowed to update their own projects.
        // If the user ID from the token does not match the project owner's ID, deny access.
        if (!project.UserId.Equals(tokenUserId))
            throw new UnauthorizedAccessException("You don't have permission to update this project background.");

        background.Title = paragraphDto.Title;
        background.Content = paragraphDto.Content;
        background.ImageUrl = paragraphDto.ImageUrl;
        background.ImageCaption = paragraphDto.ImageCaption;
        background.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
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
    public async Task DeleteProjectBackgroundParagraph(int projectId, int paragraphId, int tokenUserId)
    {
        //check if project with the given ID exists
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id.Equals(projectId))
           ?? throw new KeyNotFoundException($@"Project with ID ""{projectId}"" does not exist.");

        //Check if background paragraph with the given ID and ProjectID exists
        var background = await _context.Paragraphs
            .FirstOrDefaultAsync(p => p.Id.Equals(paragraphId) && p.ProjectId.Equals(projectId))
            ?? throw new KeyNotFoundException($@"Project background paragraph with ID ""{paragraphId}"" and ProjectId ""{projectId}"" does not exist.");

        // Compare the token User ID with the User ID of the project whose background paragraph is about to be updated
        // A user is only allowed to update their own projects.
        // If the user ID from the token does not match the project owner's ID, deny access.
        if (!project.UserId.Equals(tokenUserId))
            throw new UnauthorizedAccessException("You don't have permission to delete this project background.");

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
    public async Task AddUniqueBackgroundParagraphsAsync(int projectId, List<Paragraph> incomingParagraphs)
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
    public async Task UpdateExistingBackgroundParagraphsAsync(int projectId, List<Paragraph> incomingParagraphs)
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
