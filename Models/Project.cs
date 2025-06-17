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

        public ProjectStatus Status { get; set; } =ProjectStatus.Draft;
        public List<Paragraph> FullDescription { get; set; } = [];

        public int UserId { get; set; }

        public User? User { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    }
}
