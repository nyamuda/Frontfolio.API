
public class ChallengeDto
{
    public required int Id { get; set; }

    public required string Title { get; set; }

    public required string Problem { get; set; }

    public required string Solution { get; set; }

    public required int ProjectId { get; set; }

    public DateTime CreatedAt { get; set; }



    public static ChallengeDto MapFrom(Challenge challenge)
    {
        return new ChallengeDto
        {
            Id = challenge.Id,
            Title = challenge.Title,
            Problem = challenge.Problem,
            Solution = challenge.Solution,
            ProjectId = challenge.ProjectId,
            CreatedAt = challenge.CreatedAt

        };
    }
}

