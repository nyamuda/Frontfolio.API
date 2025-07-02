
using System.ComponentModel.DataAnnotations;

public class AddAchievementDto
{
    
    [Required]
    public required string Title { get; set; }
    [Required]
    public required string Description { get; set; }
    [Required]
    public required int ProjectId { get; set; }



    public static Achievement MapTo(AddAchievementDto achievementDto)
    {
        return new Achievement
        {      
            Title = achievementDto.Title,
            Description = achievementDto.Description,
            ProjectId = achievementDto.ProjectId,
            
        };
    }
}

