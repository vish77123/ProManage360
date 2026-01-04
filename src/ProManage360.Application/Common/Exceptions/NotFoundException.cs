namespace ProManage360.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when entity is not found
/// Returns 404 Not Found
/// </summary>
public class NotFoundException : ApplicationException
{
    public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" ({key}) was not found.")
    {
    }
}

/*
🎯 EXAMPLE USAGE:

var project = await context.Projects.FindAsync(projectId);
if (project == null)
{
    throw new NotFoundException(nameof(Project), projectId);
}

🎯 API RESPONSE (404 Not Found):
{
  "message": "Entity \"Project\" (3fa85f64-5717-4562-b3fc-2c963f66afa6) was not found."
}
*/