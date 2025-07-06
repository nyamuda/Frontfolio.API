
public static class BlogHelper
    {
    /// <summary>
    /// Ensures that the specified blog belongs to the user identified by the token.
    /// Throws an UnauthorizedAccessException if the user does not own the blog.
    /// </summary>
    /// <param name="tokenUserId">The user ID extracted from the authenticated token.</param>
    /// <param name="blog">The blog to verify ownership of.</param>
    /// <param name="crudContext">The context of the operation. Used to customize the error message</param>
    /// <exception cref="UnauthorizedAccessException">
    /// Thrown if the blog does not belong to the specified user.
    /// </exception>
    public static void EnsureUserOwnsBlog(int tokenUserId, Blog blog, CrudContext crudContext)
    {
        string errorMessage = crudContext switch
        {
            CrudContext.Create => "You don't have the permission to create this resource.",
            CrudContext.Update => "You don't have the permission to update this resource.",
            CrudContext.Delete => "You don't have the permission to delete this resource.",
            _ => "You don't have the permission to access this resource."

        };
        if (blog.UserId != tokenUserId)
            throw new UnauthorizedAccessException(errorMessage);
    }

}

