
using Frontfolio.API.Models;
using System.ComponentModel.DataAnnotations;

public class ProjectDto
{
    public required int Id { get; set; }
 
    public required string Title { get; set; }
 
    public required string Summary { get; set; }
    
    public List<string> TechStack { get; set; } = [];

    public string? GitHubUrl { get; set; }

    public string? ImageUrl { get; set; }

    public string? LiveUrl { get; set; } 

    public required ProjectStatus Status { get; set; } 

    public List<Paragraph> FullDescription { get; set; } = [];

    public int UserId { get; set; }

  

    //Map from Project to ProjectDto
    public static ProjectDto MapFrom(Project project)
    {
        return new ProjectDto
        {
            Id = project.Id,
            Title = project.Title,
            Summary = project.Summary,
            TechStack = project.TechStack,
            GitHubUrl = project.GitHubUrl,
            ImageUrl = project.ImageUrl,
            LiveUrl = project.LiveUrl,
            Status = project.Status,
            FullDescription = project.FullDescription,
            UserId = project.UserId,

        };
    }

}
