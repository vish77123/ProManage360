namespace ProManage360.Application.Common.Models;

/// <summary>
/// Result wrapper for operations that can fail
/// Alternative to throwing exceptions for expected failures
/// </summary>
public class Result
{
    public bool Succeeded { get; }
    public string[] Errors { get; }

    protected Result(bool succeeded, IEnumerable<string> errors)
    {
        Succeeded = succeeded;
        Errors = errors.ToArray();
    }

    public static Result Success()
    {
        return new Result(true, Array.Empty<string>());
    }

    public static Result Failure(IEnumerable<string> errors)
    {
        return new Result(false, errors);
    }

    public static Result Failure(string error)
    {
        return new Result(false, new[] { error });
    }
}

/// <summary>
/// Result wrapper with data
/// </summary>
public class Result<T> : Result
{
    public T? Data { get; }

    private Result(bool succeeded, T? data, IEnumerable<string> errors)
        : base(succeeded, errors)
    {
        Data = data;
    }

    public static Result<T> Success(T data)
    {
        return new Result<T>(true, data, Array.Empty<string>());
    }

    public static new Result<T> Failure(IEnumerable<string> errors)
    {
        return new Result<T>(false, default, errors);
    }

    public static new Result<T> Failure(string error)
    {
        return new Result<T>(false, default, new[] { error });
    }
}

// ============================================
// USAGE EXAMPLES
// ============================================
/*
🎯 Example 1: Simple operation result

public async Task<Result> DeleteUserAsync(Guid userId)
{
    var user = await _context.Users.FindAsync(userId);
    
    if (user == null)
        return Result.Failure("User not found");
    
    if (user.IsSystemUser)
        return Result.Failure("Cannot delete system user");
    
    _context.Users.Remove(user);
    await _context.SaveChangesAsync();
    
    return Result.Success();
}

// Usage:
var result = await DeleteUserAsync(userId);
if (!result.Succeeded)
{
    return BadRequest(result.Errors);
}
return Ok();

---

🎯 Example 2: Operation with return data

public async Task<Result<ProjectDto>> CreateProjectAsync(CreateProjectCommand command)
{
    // Validation
    if (string.IsNullOrEmpty(command.ProjectName))
        return Result<ProjectDto>.Failure("Project name is required");
    
    // Check limits
    var projectCount = await _context.Projects
        .CountAsync(p => p.TenantId == _currentUser.TenantId);
        
    if (projectCount >= tenant.MaxProjects)
        return Result<ProjectDto>.Failure("Project limit reached. Upgrade your plan.");
    
    // Create project
    var project = new Project { ... };
    _context.Projects.Add(project);
    await _context.SaveChangesAsync();
    
    var dto = _mapper.Map<ProjectDto>(project);
    return Result<ProjectDto>.Success(dto);
}

// Usage:
var result = await CreateProjectAsync(command);
if (!result.Succeeded)
{
    return BadRequest(result.Errors);
}
return Ok(result.Data); // Returns ProjectDto

---

🎯 WHEN TO USE Result vs Exceptions?

Use Result for:
✅ Expected failures (validation, business rules)
✅ When you want to return multiple errors
✅ Operations where failure is common

Use Exceptions for:
✅ Unexpected failures (database down, null reference)
✅ When you want to halt execution immediately
✅ Critical errors that should be logged

---

🎯 INTERVIEW POINT:
"We use the Result pattern for expected failures like validation 
errors or business rule violations. This makes the code more 
explicit about what can fail and why, compared to try-catch 
blocks everywhere. It also improves testability."
*/