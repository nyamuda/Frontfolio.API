
using Frontfolio.API.Models;

public class Feedback
{
    public int Id { get; set; }

    public required string AuthorName { get; set; }

    public string? AuthorRole { get; set; }

    public required string Comment { get; set; }

    public required int ProjectId { get; set; }

    public Project? Project { get; set; }

    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    public DateTime? CreatedAt { get; set; }

}

