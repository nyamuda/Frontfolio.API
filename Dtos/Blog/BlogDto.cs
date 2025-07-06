using Frontfolio.API.Models;

namespace Frontfolio.API.Dtos.Blog
{
    public class BlogDto
    {
        public required int Id { get; init; }

        public required string Title { get; set; }

        public required string Topic { get; set; }

        public required string Summary { get; set; }

        public string? ImageUrl { get; set; }

        public required BlogStatus Status { get; set; } 

        public required List<Paragraph> Content { get; set; }

        public required List<string> Tags { get; set; }

        public required int UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
