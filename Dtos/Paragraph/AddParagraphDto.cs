using Frontfolio.API.Models;
using System.ComponentModel.DataAnnotations;

public class AddParagraphDto
{
    [Required]
    public required string Title { get; set; }
    [Required]
    public required string Content { get; set; }
    [Url]
    public string? ImageUrl { get; set; } = default!;

    public string? ImageCaption { get; set; } = default!;
    

}