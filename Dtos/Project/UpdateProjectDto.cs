
using Frontfolio.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

public class UpdateProjectDto
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }

    /// <summary>
    /// Determines the custom display order of projects in the UI.
    /// Lower values appear first.
    /// </summary>

    [Required]
    public int SortOrder { get; set; }

    [Required]
    public ProjectDifficultyLevel DifficultyLevel { get; set; }

    [Required]
    public string Summary { get; set; }
    [Required]
    public DateTime StartDate { get; set; }
    [Required]
    public DateTime EndDate { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "You need to include at least one tool, language, or framework used in this project.")]
    public List<string> TechStack { get; set; } = [];
    [Url]
    public string? GitHubUrl { get; set; }
    [Url]
    public string? ImageUrl { get; set; }
    [Url]
    public string? VideoUrl { get; set; }
    [Url]
    public string? LiveUrl { get; set; }

    [Required]
    public ProjectStatus Status { get; set; }


    public List<UpdateParagraphDto> Background { get; set; } = [];

    public List<UpdateChallengeDto> Challenges { get; set; } = [];

    public List<UpdateAchievementDto> Achievements { get; set; } = [];

    public List<UpdateFeedbackDto> Feedback { get; set; } = [];



    //Map AddProjectDto to Project
    public static Project MapTo(UpdateProjectDto projectDto)
    {
        return new Project
        {
            Title = projectDto.Title,
            SortOrder = projectDto.SortOrder,
            DifficultyLevel = projectDto.DifficultyLevel,
            StartDate = projectDto.StartDate,
            EndDate = projectDto.EndDate,
            Summary = projectDto.Summary,
            TechStack = projectDto.TechStack,
            GitHubUrl = projectDto.GitHubUrl,
            ImageUrl = projectDto.ImageUrl,
            VideoUrl = projectDto.VideoUrl,
            LiveUrl = projectDto.LiveUrl,
            Status = projectDto.Status,
            Background = projectDto.Background.Select(p => new Paragraph
            {
                Id=p.Id,
                Title = p.Title,
                Content = p.Content,
                ImageUrl = p.ImageUrl,
                ImageCaption = p.ImageCaption,
                ParagraphType = ParagraphType.ProjectBackground
            }).ToList(),
            Challenges = projectDto.Challenges.Select(c =>UpdateChallengeDto.MapTo(c)).ToList(),
            Achievements = projectDto.Achievements.Select(a =>UpdateAchievementDto.MapTo(a)).ToList(),
            Feedback = projectDto.Feedback.Select(f =>UpdateFeedbackDto.MapTo(f)).ToList(),
            UpdatedAt = DateTime.UtcNow

        };

    }
}

