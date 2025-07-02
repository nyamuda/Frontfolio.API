
using System.ComponentModel.DataAnnotations;

public class UpdateChallengeDto
    {
    [Required]
    public required int Id { get; set; }
    [Required]
    public required string Title { get; set; }
    [Required]
    public required string Problem { get; set; }
    [Required]
    public required string Solution { get; set; }
    [Required]
    public required int ProjectId { get; set; }




    public static Challenge MapTo(UpdateChallengeDto challengeDto)
    {
        return new Challenge
        {
            Id = challengeDto.Id,
            Title = challengeDto.Title,
            Problem = challengeDto.Problem,
            Solution = challengeDto.Solution,
            ProjectId = challengeDto.ProjectId,
            UpdatedAt = DateTime.UtcNow

        };
    }
}

