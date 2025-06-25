
using Frontfolio.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

public class UpdateProjectDto
{
    
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


    public List<Paragraph> Background { get; set; } = [];

    public List<Challenge> Challenges { get; set; } = [];

    public List<Achievement> Achievements { get; set; } = [];

    public List<Feedback> Feedback { get; set; } = [];



    //Map AddProjectDto to Project
    public static Project MapTo(UpdateProjectDto projectDto)
    {
        return new Project
        {
            Title = projectDto.Title,
            SortOrder=projectDto.SortOrder,
            DifficultyLevel=projectDto.DifficultyLevel,
            StartDate=projectDto.StartDate,
            EndDate=projectDto.EndDate,
            Summary = projectDto.Summary,
            TechStack = projectDto.TechStack,
            GitHubUrl = projectDto.GitHubUrl,
            ImageUrl = projectDto.ImageUrl,
            VideoUrl=projectDto.VideoUrl,                   
            LiveUrl = projectDto.LiveUrl,
            Status = projectDto.Status,
            Background = projectDto.Background,
            Challenges = projectDto.Challenges,
            Achievements = projectDto.Achievements,
            Feedback = projectDto.Feedback,

        };

    }
}

