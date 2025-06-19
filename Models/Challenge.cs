namespace Frontfolio.API.Models
{
    public class Challenge
    {
        public int Id { get; set; }

        public required string Title { get; set; }

        public required string Problem { get; set; }

        public required string Solution { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; } 


    }
}
