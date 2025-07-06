using Frontfolio.API.Data;
using Frontfolio.API.Models;
using Frontfolio.API.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Frontfolio.API.Services
{
    public class BlogParagraphService:IParagraphService
    {

        private readonly ApplicationDbContext _context;


        public BlogParagraphService(ApplicationDbContext context)
        {
            _context = context;


        }

        /// <summary>
        /// Deletes a paragraph for a specific blog.
        /// </summary>
        /// <param name="blogId">The ID of the blog to which the paragraph belongs.</param>
        /// <param name="paragraphId">The ID of the paragraph to delete.</param>
        /// <param name="tokenUserId">The ID of the user making the request, extracted from the JWT token.</param>
        /// <exception cref="KeyNotFoundException">
        /// Thrown when the blog or the specified paragraph cannot be found.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// Thrown when the user attempting the delete is not the owner of the blog.
        /// </exception>
        /// <remarks>
        /// This method ensures that only the owner of the blog can delete its paragraph. 
        /// It validates the existence of both the blog and paragraph before deletion.
        /// </remarks>
        public async Task DeleteByIdAsync(int blogId, int paragraphId, int tokenUserId)
        {
            //check if blog with the given ID exists
            var blog = await _context.Blogs.FirstOrDefaultAsync(p => p.Id.Equals(blogId))
               ?? throw new KeyNotFoundException($@"Blog with ID ""{blogId}"" does not exist.");

            //Check paragraph with the given ID and BlogId exists
            var background = await _context.Paragraphs
                .FirstOrDefaultAsync(p => p.Id.Equals(paragraphId) && p.BlogId.Equals(blogId))
                ?? throw new KeyNotFoundException($@"Paragraph with ID ""{paragraphId}"" and BlogId ""{blogId}"" does not exist.");


            //Only the owner the blog is allowed to delete its paragraph
            BlogHelper.EnsureUserOwnsBlog(tokenUserId, blog, crudContext: CrudContext.Delete);

            _context.Paragraphs.Remove(background);
            await _context.SaveChangesAsync();
        }

         
        /// <summary>
        /// Adds new paragraphs to a blog by excluding any paragraphs
        /// that already exist in the blog's current paragraph list.
        /// </summary>
        /// <param name="blogId">The ID of the blog to update.</param>
        /// <param name="incomingParagraphs">The full list of incoming paragraphs (both new and possibly existing ones).</param>
        /// <exception cref="KeyNotFoundException">Thrown if a blog with the given ID is not found.</exception>
        public async Task AddIfNotExistingAsync(int blogId, List<Paragraph> incomingParagraphs)
        {
            // Retrieve the existing blog with the given ID
            var blog = await _context.Blogs.FirstOrDefaultAsync(p => p.Id == blogId)
                ?? throw new KeyNotFoundException($@"Blog with ID ""{blogId}"" does not exist.");

            // Get a list of IDs for paragraphs that are already part of the blog
            List<int> existingParagraphIds = blog.Content.Select(p => p.Id).ToList();

            // Filter incoming paragraphs to exclude any that already exist
            var uniqueParagraphs = incomingParagraphs
                .Where(p => !existingParagraphIds.Contains(p.Id))
                .ToList();

            // Add the unique paragraphs to the blog
            blog.Content.AddRange(uniqueParagraphs);

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates existing background paragraphs for a given blog by applying any changes
        /// from the incoming list of paragraphs that match by ID.
        /// </summary>
        /// <param name="blogId">The ID of the blog whose paragraphs should be updated.</param>
        /// <param name="incomingParagraphs">The list of paragraphs containing updated content.</param>
        /// <exception cref="KeyNotFoundException">Thrown if the blog with the specified ID is not found.</exception>
        public async Task UpdateExistingAsync(int blogId, List<Paragraph> incomingParagraphs)
        {
            // Retrieve the blog including its current background paragraphs
            var blog = await _context.Blogs
                .Include(p => p.Background)
                .FirstOrDefaultAsync(p => p.Id == blogId)
                ?? throw new KeyNotFoundException($@"Blog with ID ""{blogId}"" does not exist.");

            // Loop through each existing paragraph and try to find a match in the incoming list
            foreach (var existingParagraph in blog.Background)
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
}
