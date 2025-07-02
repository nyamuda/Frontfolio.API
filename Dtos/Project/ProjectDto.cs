
using Frontfolio.API.Models;
using System.ComponentModel.DataAnnotations;

public class ProjectDto
{
    public required int Id { get; set; }
 
    public required string Title { get; set; }

    public ProjectDifficultyLevel DifficultyLevel { get; set; }

    /// <summary>
    /// Determines the custom display order of projects in the UI.
    /// Lower values appear first.
    /// </summary>
    public int SortOrder { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public required string Summary { get; set; }
    
    public List<string> TechStack { get; set; } = [];

    public string? GitHubUrl { get; set; }

    public string? ImageUrl { get; set; }

    public string? VideoUrl { get; set; }

    public string? LiveUrl { get; set; } 

    public required ProjectStatus Status { get; set; }

    public List<ParagraphDto> Background { get; set; } = [];

    public List<ChallengeDto> Challenges { get; set; } = [];

    public List<AchievementDto> Achievements { get; set; } = [];

    public List<FeedbackDto> Feedback { get; set; } = [];

    public int UserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }



    //Map from Project to ProjectDto
    public static ProjectDto MapFrom(Project project)
    {
        return new ProjectDto
        {
            Id = project.Id,
            Title = project.Title,
            SortOrder=project.SortOrder,
            DifficultyLevel=project.DifficultyLevel,
            StartDate=project.StartDate,
            EndDate=project.EndDate,
            Summary = project.Summary,
            TechStack = project.TechStack,
            GitHubUrl = project.GitHubUrl,
            ImageUrl = project.ImageUrl,
            VideoUrl=project.VideoUrl,
            LiveUrl = project.LiveUrl,
            Status = project.Status,
            Background = project.Background.Select(p => ParagraphDto.MapFrom(p)).ToList(),
            Challenges=project.Challenges.Select(c => ChallengeDto.MapFrom(c)).ToList(),
            Achievements=project.Achievements.Select(a =>AchievementDto.MapFrom(a)).ToList(),
            Feedback=project.Feedback.Select(f =>FeedbackDto.MapFrom(f)).ToList(),
            UserId = project.UserId,
            CreatedAt=project.CreatedAt,
            UpdatedAt=project.UpdatedAt

        };
    }

    
  

}
