using Frontfolio.API.Models;

public class ProjectHelper
{

    /// <summary>
    /// Ensures that the specified project belongs to the user identified by the token.
    /// Throws an UnauthorizedAccessException if the user does not own the project.
    /// </summary>
    /// <param name="tokenUserId">The user ID extracted from the authenticated token.</param>
    /// <param name="project">The project to verify ownership of.</param>
    /// <exception cref="UnauthorizedAccessException">
    /// Thrown if the project does not belong to the specified user.
    /// </exception>
    public static void EnsureUserOwnsProject(int tokenUserId, Project project,string errorMessage)
    {
        if (project.UserId != tokenUserId)
            throw new UnauthorizedAccessException(errorMessage);
    }




}

