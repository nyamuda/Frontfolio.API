namespace Frontfolio.API.Models
{
    public class Project
    {
        public int Id { get; set; }

        public required string Title { get; set; }

        public required string Summary { get; set; }

        public List<string> TechStack { get; set; } = [];

        public string GitHubUrl { get; set; } = default!;

        public string ImageUrl { get; set; } = default!;

        public string LiveUrl { get; set; } = default!;

        public List<Paragraph> Description { get; set; } = [];

        public List<Paragraph> Challenges { get; set; } = [];

        public List<Paragraph> Achievements { get; set; } = [];


    }
}
