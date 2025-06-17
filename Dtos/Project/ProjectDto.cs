
using Frontfolio.API.Models;
using System.ComponentModel.DataAnnotations;

public class ProjectDto
{
    public int Id { get; set; }
 
    public required string Title { get; set; }
 
    public required string Summary { get; set; }
    
    public List<string> TechStack { get; set; } = [];

    public string? GitHubUrl { get; set; }

    public string? ImageUrl { get; set; }

    public string? LiveUrl { get; set; } 

    public required ProjectStatus Status { get; set; } 

    public List<Paragraph> FullDescription { get; set; } = [];

}
