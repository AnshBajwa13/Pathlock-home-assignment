using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Tasks.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Tasks.Queries.GetTasksByProject;

/// <summary>
/// Handler for getting tasks by project ID
/// </summary>
public class GetTasksByProjectQueryHandler : IRequestHandler<GetTasksByProjectQuery, Result<List<TaskDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetTasksByProjectQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<List<TaskDto>>> Handle(GetTasksByProjectQuery request, CancellationToken cancellationToken)
    {
        // Get current user ID
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            return Result<List<TaskDto>>.Failure("User not authenticated");
        }

        // Verify project exists and user owns it
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken);

        if (project == null)
        {
            return Result<List<TaskDto>>.Failure("Project not found");
        }

        if (project.UserId != userId.Value)
        {
            return Result<List<TaskDto>>.Failure("You do not have permission to view tasks for this project");
        }

        // Build query
        var query = _context.Tasks
            .Where(t => t.ProjectId == request.ProjectId);

        // Apply optional completion filter
        if (request.IsCompleted.HasValue)
        {
            query = query.Where(t => t.IsCompleted == request.IsCompleted.Value);
        }

        // Get tasks
        var tasks = await query
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new TaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                IsCompleted = t.IsCompleted,
                CompletedAt = t.CompletedAt,
                DueDate = t.DueDate,
                ProjectId = t.ProjectId,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                EstimatedHours = t.EstimatedHours,
                DependencyIds = _context.TaskDependencies
                    .Where(td => td.TaskId == t.Id)
                    .Select(td => td.DependsOnTaskId)
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        return Result<List<TaskDto>>.Success(tasks);
    }
}
