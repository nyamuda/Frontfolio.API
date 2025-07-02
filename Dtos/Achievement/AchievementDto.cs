
public class AchievementDto
    {
    
    public required int Id { get; set; }
 
    public required string Title { get; set; }
  
    public required string Description { get; set; }
   
    public required int ProjectId { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; } 



    public static AchievementDto MapFrom(Achievement achievement)
    {
        return new AchievementDto
        {
            Id = achievement.Id,
            Title = achievement.Title,
            Description = achievement.Description,
            ProjectId = achievement.ProjectId,
            UpdatedAt = achievement.UpdatedAt,
            CreatedAt=achievement.CreatedAt
        };
    }

}

