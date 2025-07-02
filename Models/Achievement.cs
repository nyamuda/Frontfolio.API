
using Frontfolio.API.Models;

public class Achievement
{
    public int Id { get; set; }

    public required string Title { get; set; }

    public required string Description { get; set; }

    public required int ProjectId { get; set; }

    public Project? Project { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; }=DateTime.UtcNow;

}

