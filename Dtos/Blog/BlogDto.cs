using Frontfolio.API.Models;

public class BlogDto
{
    public required int Id { get; init; }

    public required string Title { get; set; }

    public required string Topic { get; set; }

    public required string Summary { get; set; }

    public string? ImageUrl { get; set; }

    public required BlogStatus Status { get; set; }

    public required List<ParagraphDto> Content { get; set; }

    public required List<string> Tags { get; set; }

    public required int UserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public static BlogDto MapFrom(Blog blog)
    {
        return new BlogDto
        {
            Id = blog.Id,
            Title = blog.Title,
            Topic = blog.Topic,
            Summary = blog.Summary,
            ImageUrl = blog.ImageUrl,
            Status = blog.Status,
            Content = blog.Content.Select(p => ParagraphDto.MapFrom(p)).ToList(),
            Tags = blog.Tags,
            UserId = blog.UserId,
            CreatedAt = blog.CreatedAt,
            PublishedAt = blog.PublishedAt,
            UpdatedAt = blog.UpdatedAt,

        };
    }
}



