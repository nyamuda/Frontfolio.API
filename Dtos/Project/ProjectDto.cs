
using Frontfolio.API.Models;
using System.ComponentModel.DataAnnotations;

public class ProjectDto
{
    public required int Id { get; set; }
 
    public required string Title { get; set; }

    /// <summary>
    /// Determines the custom display order of projects in the UI.
    /// Lower values appear first.
    /// </summary>
    public int? SortOrder { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public required string Summary { get; set; }
    
    public List<string> TechStack { get; set; } = [];

    public string? GitHubUrl { get; set; }

    public string? ImageUrl { get; set; }

    public string? VideoUrl { get; set; }

    public string? LiveUrl { get; set; } 

    public required ProjectStatus Status { get; set; }

    public List<Paragraph> Background { get; set; } = [];

    public List<Challenge> Challenges { get; set; } = [];

    public List<Achievement> Achievements { get; set; } = [];

    public List<Feedback> Feedback { get; set; } = [];

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
            StartDate=project.StartDate,
            EndDate=project.EndDate,
            Summary = project.Summary,
            TechStack = project.TechStack,
            GitHubUrl = project.GitHubUrl,
            ImageUrl = project.ImageUrl,
            VideoUrl=project.VideoUrl,
            LiveUrl = project.LiveUrl,
            Status = project.Status,
            Background = project.Background,
            Challenges=project.Challenges,
            Achievements=project.Achievements,
            Feedback=project.Feedback,
            UserId = project.UserId,
            CreatedAt=project.CreatedAt,
            UpdatedAt=project.UpdatedAt

        };
    }

    //Map ProjectDto to Project
    public static Project MapTo(ProjectDto projectDto)
    {
        return new Project
        {
            Title = projectDto.Title,
            SortOrder=projectDto.SortOrder,
            Summary = projectDto.Summary,
            TechStack = projectDto.TechStack,
            GitHubUrl = projectDto.GitHubUrl,
            ImageUrl = projectDto.ImageUrl,
            LiveUrl = projectDto.LiveUrl,
            Status = projectDto.Status,
            Background = projectDto.Background,
            Challenges = projectDto.Challenges,
            Achievements = projectDto.Achievements,
            Feedback = projectDto.Feedback,

        };
    }

}
