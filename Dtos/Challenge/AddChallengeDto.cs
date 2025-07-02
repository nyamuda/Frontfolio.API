
using System.ComponentModel.DataAnnotations;

public class AddChallengeDto
{

    [Required]
    public required string Title { get; set; }
    [Required]
    public required string Problem { get; set; }
    [Required]
    public required string Solution { get; set; }
    [Required]
    public required int ProjectId { get; set; }



    public static Challenge MapTo(AddChallengeDto challengeDto)
    {
        return new Challenge
        {
            Title = challengeDto.Title,
            Problem = challengeDto.Problem,
            Solution = challengeDto.Solution,
            ProjectId = challengeDto.ProjectId,

        };
    }
}

