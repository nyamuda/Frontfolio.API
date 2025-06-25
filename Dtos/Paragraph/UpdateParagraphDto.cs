using Frontfolio.API.Models;

public class UpdateParagraphDto
{
    public required string Title { get; set; }
    public required string Content { get; set; }
    public string ImageUrl { get; set; } = default!;
    public string ImageCaption { get; set; } = default!;
    public required ParagraphType ParagraphType { get; set; }
    public required int ProjectId { get; set; }
 



}