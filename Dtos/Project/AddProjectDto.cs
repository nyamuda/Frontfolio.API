
using Frontfolio.API.Models;
using System.ComponentModel.DataAnnotations;

public class AddProjectDto
{

    [Required]
    public string Title { get; set; }

    [Required]
    public string Summary { get; set; }

    [Required]
    [MinLength(1)]
    public List<string> TechStack { get; set; } = [];
    [Url]
    public string? GitHubUrl { get; set; }
    [Url]
    public string? ImageUrl { get; set; }
    [Url]
    public string? LiveUrl { get; set; }

    [Required]
    public ProjectStatus Status { get; set; }


    public List<Paragraph> Background { get; set; } = [];

    public List<Challenge> Challenges { get; set; } = [];

    public List<Achievement> Achievements { get; set; } = [];

    public List<Feedback> Feedback { get; set; } = [];



    //Map AddProjectDto to Project
    public static Project MapTo(AddProjectDto projectDto)
    {
        return new Project
        {
            Title = projectDto.Title,
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

