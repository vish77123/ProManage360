namespace ProManage360.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when user doesn't have permission
/// Returns 403 Forbidden
/// </summary>
public class ForbiddenAccessException : ApplicationException
{
    public ForbiddenAccessException()
        : base("You do not have permission to perform this action.")
    {
    }

    public ForbiddenAccessException(string message)
        : base(message)
    {
    }
}

/*
🎯 EXAMPLE USAGE:

var project = await context.Projects.FindAsync(projectId);
if (project.OwnerId != currentUser.UserId)
{
    throw new ForbiddenAccessException("Only project owner can delete this project.");
}

🎯 API RESPONSE (403 Forbidden):
{
  "message": "Only project owner can delete this project."
}
*/