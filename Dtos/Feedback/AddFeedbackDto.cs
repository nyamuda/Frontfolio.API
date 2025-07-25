﻿
using System.ComponentModel.DataAnnotations;

public class AddFeedbackDto
{

    [Required]
    public required string AuthorName { get; set; }

    public string? AuthorRole { get; set; }
    [Required]
    public required string Comment { get; set; }
    [Required]
    public required int ProjectId { get; set; }
    [Required]
    public required DateTime SubmittedAt { get; set; }



    public static Feedback MapTo(AddFeedbackDto feedbackDto)
    {
        return new Feedback
        {          
            AuthorName = feedbackDto.AuthorName,
            AuthorRole = feedbackDto.AuthorRole,
            Comment = feedbackDto.Comment,
            ProjectId = feedbackDto.ProjectId,
            SubmittedAt = TimeZoneInfo.ConvertTimeToUtc(feedbackDto.SubmittedAt),

        };
    }

}

