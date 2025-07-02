using Frontfolio.API.Models;
using System.ComponentModel.DataAnnotations;

public class AddParagraphDto
{
    [Required]
    public required string Title { get; set; }
    [Required]
    public required string Content { get; set; }

    [Required]
    public required ParagraphType ParagraphType { get; set; }

    [Url]
    public string? ImageUrl { get; set; } = default!;

    public string? ImageCaption { get; set; } = default!;

    public static Paragraph MapTo(AddParagraphDto paragraphDto)
    {
        return new Paragraph
        {         
            Title = paragraphDto.Title,
            Content = paragraphDto.Content,
            ImageUrl = paragraphDto.ImageUrl,
            ImageCaption = paragraphDto.ImageCaption,
            ParagraphType = paragraphDto.ParagraphType,          
        };

    }


}