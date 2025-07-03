using Frontfolio.API.Models;

public class ProjectHelper
{

    /// <summary>
    /// Ensures that the specified project belongs to the user identified by the token.
    /// Throws an UnauthorizedAccessException if the user does not own the project.
    /// </summary>
    /// <param name="tokenUserId">The user ID extracted from the authenticated token.</param>
    /// <param name="project">The project to verify ownership of.</param>
    /// <param name="crudContext">The context of the operation. Used to customize the error message</param>
    /// <exception cref="UnauthorizedAccessException">
    /// Thrown if the project does not belong to the specified user.
    /// </exception>
    public static void EnsureUserOwnsProject(int tokenUserId, Project project,CrudContext crudContext)
    {
        string errorMessage = crudContext switch
        {
            CrudContext.Create => "You don't have the permission to create this resource",
            CrudContext.Update => "You don't have the permission to update this resource",
            CrudContext.Delete => "You don't have the permission to delete this resource",
            _ => "You don't have the permission to access this resource"

        };
        if (project.UserId != tokenUserId)
            throw new UnauthorizedAccessException(errorMessage);
    }




}

