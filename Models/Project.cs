namespace Frontfolio.API.Models
{
    public class Project
    {
        public int Id { get; set; }

        public required string Title { get; set; }

        public required string Summary { get; set; }

        public List<string> TechStack { get; set; } = [];

        public required string GitHubUrl { get; set; }

        public required string ImageUrl { get; set; }

        public required string LiveUrl { get; set; }


    }
}
