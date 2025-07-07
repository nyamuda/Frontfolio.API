using Frontfolio.API.Models;

public class ParagraphDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public string? ImageUrl { get; set; } = default!;
    public string? ImageCaption { get; set; } = default!;
    public required ParagraphType ParagraphType { get; set; }
    public int? ProjectId { get; set; }
    public int? BlogId { get; set; }
    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set;}  


    public static ParagraphDto MapFrom(Paragraph paragraph)
    {
        return new ParagraphDto
        {
            Id = paragraph.Id,
            Title = paragraph.Title,
            Content = paragraph.Content,
            ImageUrl = paragraph.ImageUrl,
            ImageCaption = paragraph.ImageCaption,
            ParagraphType = paragraph.ParagraphType,
            ProjectId = paragraph.ProjectId,
            BlogId = paragraph.BlogId,
            CreatedAt =paragraph.CreatedAt,
            UpdatedAt=paragraph.UpdatedAt,
        };


    }
}