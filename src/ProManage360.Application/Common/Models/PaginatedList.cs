namespace ProManage360.Application.Common.Models;

using Microsoft.EntityFrameworkCore;

/// <summary>
/// Generic paginated list with metadata
/// Used for all paginated queries
/// </summary>
public class PaginatedList<T>
{
    public List<T> Items { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public int TotalPages { get; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public PaginatedList(List<T> items, int count, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = count;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
    }

    /// <summary>
    /// Create paginated list from IQueryable
    /// </summary>
    public static async Task<PaginatedList<T>> CreateAsync(
        IQueryable<T> source,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        // Get total count (before paging)
        var count = await source.CountAsync(cancellationToken);

        // Get items for current page
        var items = await source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }
}

// ============================================
// USAGE EXAMPLE
// ============================================
/*
🎯 In Query Handler:

public class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, PaginatedList<ProjectDto>>
{
    public async Task<PaginatedList<ProjectDto>> Handle(...)
    {
        var query = _context.Projects
            .Where(p => p.Status == ProjectStatus.Active)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new ProjectDto
            {
                ProjectId = p.ProjectId,
                ProjectName = p.ProjectName,
                // ... map other properties
            });

        // Automatically handles Skip/Take and TotalCount
        return await PaginatedList<ProjectDto>.CreateAsync(
            query, 
            pageNumber: 1, 
            pageSize: 10);
    }
}

🎯 API RESPONSE:
{
  "items": [
    { "projectId": "...", "projectName": "Project 1" },
    { "projectId": "...", "projectName": "Project 2" }
  ],
  "pageNumber": 1,
  "pageSize": 10,
  "totalCount": 45,
  "totalPages": 5,
  "hasPreviousPage": false,
  "hasNextPage": true
}

🎯 INTERVIEW POINT:
"We use a generic PaginatedList<T> to standardize pagination 
across all queries. It provides client with metadata needed 
for building pagination UI."
*/