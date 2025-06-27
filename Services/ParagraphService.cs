
using Frontfolio.API.Data;
using Frontfolio.API.Models;
using Microsoft.EntityFrameworkCore;

public class ParagraphService
{

    private readonly ApplicationDbContext _context;


    public ParagraphService(ApplicationDbContext context)
    {
        _context = context;
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
    public async Task<List<ParagraphDto>> GetProjectBackgroundParagraphs(int projectId)
    {
        return await _context.Paragraphs
           .Where(p => p.ProjectId.Equals(projectId))
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
    /// <param name="backgroundId">The ID of the background paragraph to update.</param>
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
    public async Task UpdateProjectBackgroundParagraph(int projectId, int backgroundId, int tokenUserId, UpdateParagraphDto paragraphDto)
    {
        //check if project with the given ID exists
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id.Equals(projectId))
           ?? throw new KeyNotFoundException($@"Project with ID ""{projectId}"" does not exist.");

        //Check if background paragraph with the given ID and ProjectID exists
        var background = await _context.Paragraphs
            .FirstOrDefaultAsync(p => p.Id.Equals(backgroundId) && p.ProjectId.Equals(projectId))
            ?? throw new KeyNotFoundException($@"Project background with ID ""{backgroundId}"" and ProjectID ""{projectId}"" does not exist.");

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
    /// <param name="backgroundId">The ID of the background paragraph to delete.</param>
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
    public async Task DeleteBackgroundParagraphForProject(int projectId, int backgroundId, int tokenUserId)
    {
        //check if project with the given ID exists
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id.Equals(projectId))
           ?? throw new KeyNotFoundException($@"Project with ID ""{projectId}"" does not exist.");

        //Check if background paragraph with the given ID and ProjectID exists
        var background = await _context.Paragraphs
            .FirstOrDefaultAsync(p => p.Id.Equals(backgroundId) && p.ProjectId.Equals(projectId))
            ?? throw new KeyNotFoundException($@"Project background with ID ""{backgroundId}"" and ProjectID ""{projectId}"" does not exist.");

        // Compare the token User ID with the User ID of the project whose background paragraph is about to be updated
        // A user is only allowed to update their own projects.
        // If the user ID from the token does not match the project owner's ID, deny access.
        if (!project.UserId.Equals(tokenUserId))
            throw new UnauthorizedAccessException("You don't have permission to delete this project background.");

        _context.Paragraphs.Remove(background);
        await _context.SaveChangesAsync();
    }

}
