using Frontfolio.API.Models;
using System.ComponentModel.DataAnnotations;

public class UpdateParagraphDto
{
    [Required]
    public required string Title { get; set; }
    [Required]
    public required string Content { get; set; }
    [Url]
    public string? ImageUrl { get; set; } = default!;

    public string? ImageCaption { get; set; } = default!;
    [Required]
    public required ParagraphType ParagraphType { get; set; }
    [Required]
    public required int ProjectId { get; set; }
 


    public static Paragraph MapTo(UpdateParagraphDto paragraphDto)
    {
        return new Paragraph
        {
            Title = paragraphDto.Title,
            Content = paragraphDto.Content,
            ImageUrl = paragraphDto.ImageUrl,
            ImageCaption = paragraphDto.ImageCaption,
            ParagraphType = paragraphDto.ParagraphType,
            ProjectId = paragraphDto.ProjectId,

        };
    }



}