
using Frontfolio.API.Models;

public class Blog
{

    public int Id { get; init; }

    public required string Title { get; set; }

    public required string Topic { get; set; }

    public required string Summary { get; set; }

    public string? ImageUrl { get; set; }

    public required BlogStatus Status { get; set; } = BlogStatus.Draft;

    public required List<Paragraph> Content { get; set; }

    public required List<string> Tags { get; set; } 

    public required int UserId { get; set; }

    public User? User { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

}

