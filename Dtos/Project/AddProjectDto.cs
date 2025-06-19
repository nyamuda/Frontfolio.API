
using System.ComponentModel.DataAnnotations;

public class AddProjectDto
    {

    [Required]
    public required string Title { get; set; }

    [Required]
    public required string Summary { get; set; }

    [Required]
    [MinLength(1)]
    public List<string> TechStack { get; set; } = [];

    public string? GitHubUrl { get; set; }

    public string? ImageUrl { get; set; }

    public string? LiveUrl { get; set; }

    public required ProjectStatus Status { get; set; } = ProjectStatus.Draft;

    public List<ParagraphType> Description { get; set; } = [];


}

