
    public class UpdateChallengeDto
    {
    public required int Id { get; set; }

    public required string Title { get; set; }

    public required string Problem { get; set; }

    public required string Solution { get; set; }

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
           

        };
    }
}

