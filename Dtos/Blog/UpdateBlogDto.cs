
using System.ComponentModel.DataAnnotations;

public class UpdateBlogDto
{

    [Required]
    public required string Title { get; set; }
    [Required]
    public required string Topic { get; set; }
    [Required]
    public required string Summary { get; set; }
    [Url]
    public string? ImageUrl { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "Your content must include at least one paragraph.")]
    public required List<UpdateParagraphDto> Content { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "Please add at least one tag.")]
    public required List<string> Tags { get; set; }


    public static Blog MapTo(UpdateBlogDto blogDto)
    {
        return new Blog
        {
            Title = blogDto.Title,
            Topic = blogDto.Topic,
            Summary = blogDto.Summary,
            ImageUrl = blogDto.ImageUrl,
            Content = blogDto.Content.Select(p => UpdateParagraphDto.MapTo(p)).ToList(),
            Tags = blogDto.Tags,
            UpdatedAt= DateTime.UtcNow

        };
    }

}

