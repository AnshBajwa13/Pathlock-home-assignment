using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Projects.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Projects.Queries.GetProjects;

/// <summary>
/// Handler for getting all projects for the current user
/// </summary>
public class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, Result<PaginatedList<ProjectDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetProjectsQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<PaginatedList<ProjectDto>>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
    {
        // Get current user ID
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            return Result<PaginatedList<ProjectDto>>.Failure("User not authenticated");
        }

        // Query projects for current user (soft delete filter is automatic via query filter)
        var query = _context.Projects
            .Include(p => p.Tasks)
            .Where(p => p.UserId == userId.Value)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new ProjectDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                TaskCount = p.Tasks.Count,
                CompletedTaskCount = p.Tasks.Count(t => t.IsCompleted),
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            });

        // Get paginated list
        var paginatedList = await PaginatedList<ProjectDto>.CreateAsync(
            query,
            request.PageNumber,
            request.PageSize);

        return Result<PaginatedList<ProjectDto>>.Success(paginatedList);
    }
}
