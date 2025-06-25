namespace Frontfolio.API.Models
{
    public class Project
    {
        public int Id { get; set; }

        public required string Title { get; set; }


        public int OrderLevel { get; set; } 

        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        public DateTime EndDate { get; set; } = DateTime.UtcNow;

        public required string Summary { get; set; }

        public List<string> TechStack { get; set; } = [];

        public string? GitHubUrl { get; set; }

        public string? ImageUrl { get; set; } 

        public string? VideoUrl { get; set; }

        public string? LiveUrl { get; set; } 

        public ProjectStatus Status { get; set; } =ProjectStatus.Draft;

        public List<Paragraph> Background { get; set; } = [];

        public List<Challenge> Challenges { get; set; } = [];

        public List<Achievement> Achievements { get; set; } = [];

        public List<Feedback> Feedback { get; set; } = [];

        public int UserId { get; set; }

        public User? User { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }


    }
}
