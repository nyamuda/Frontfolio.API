
using Frontfolio.API.Models;

public class FeedbackDto
{
    public required int Id { get; set; }

    public required string AuthorName { get; set; }

    public string? AuthorRole { get; set; }

    public required string Comment { get; set; }

    public required int ProjectId { get; set; }

    public required DateTime SubmittedAt { get; set; } 

    public DateTime? CreatedAt { get; set; }


    public static FeedbackDto MapFrom(Feedback feedback)
    {
        return new FeedbackDto
        {
            Id = feedback.Id,
            AuthorName = feedback.AuthorName,
            AuthorRole = feedback.AuthorRole,
            Comment = feedback.Comment,
            ProjectId = feedback.ProjectId,
            SubmittedAt = feedback.SubmittedAt
        };
    }
}

