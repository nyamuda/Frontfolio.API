
using System.ComponentModel.DataAnnotations;

public class UpdateFeedbackDto
{
    [Required]
    public int Id { get; set; }

    [Required]
    public required string AuthorName { get; set; }
    
    public string? AuthorRole { get; set; }
    [Required]
    public required string Comment { get; set; }
    [Required]
    public required int ProjectId { get; set; }
    [Required]
    public required DateTime SubmittedAt { get; set; }



    public static Feedback MapTo(UpdateFeedbackDto feedbackDto)
    {
        return new Feedback
        {
            Id = feedbackDto.Id,
            AuthorName = feedbackDto.AuthorName,
            AuthorRole = feedbackDto.AuthorRole,
            Comment = feedbackDto.Comment,
            ProjectId = feedbackDto.ProjectId,
            SubmittedAt = TimeZoneInfo.ConvertTimeToUtc(feedbackDto.SubmittedAt),
            UpdatedAt = DateTime.UtcNow

        };
    }
}
