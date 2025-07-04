﻿
public static class ErrorMessageHelper
{

    public static string UnexpectedErrorMessage() =>
        "The server encountered an unexpected issue. Please try again later.";

    public static string ForbiddenErrorMessage() =>
        "You do not have permission to access this resource.";

    public static string InvalidNameIdentifierMessage() =>
        "Access denied. Token lacks a valid name identifier claim.";


}
