namespace Frontfolio.API.Models
{
    public class Paragraph
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
        public string ImageUrl { get; set; } = default!;
        public string ImageCaption { get; set; } = default!;
        public int ProjectId { get; set; }  
        public Project? Project { get; set; }
    }
}
