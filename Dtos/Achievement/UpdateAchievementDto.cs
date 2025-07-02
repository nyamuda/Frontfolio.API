
using Frontfolio.API.Models;
using System.ComponentModel.DataAnnotations;

public class UpdateAchievementDto
{
    [Required]
    public required int Id { get; set; }

    [Required]
    public required string Title { get; set; }
    [Required]
    public required string Description { get; set; }
    [Required]
    public required int ProjectId { get; set; }



    public static Achievement MapTo(UpdateAchievementDto achievementDto)
    {
        return new Achievement
        {
            Id = achievementDto.Id,
            Title = achievementDto.Title,
            Description = achievementDto.Description,
            ProjectId = achievementDto.ProjectId,
            UpdatedAt=DateTime.UtcNow
        };
    }


}

