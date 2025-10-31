using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Tasks.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Tasks.Queries.GetTaskById;

/// <summary>
/// Handler for getting a task by ID
/// </summary>
public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, Result<TaskDetailDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetTaskByIdQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<TaskDetailDto>> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        // Get current user ID
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            return Result<TaskDetailDto>.Failure("User not authenticated");
        }

        // Find task with project
        var task = await _context.Tasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (task == null)
        {
            return Result<TaskDetailDto>.Failure("Task not found");
        }

        // Check ownership (user must own the project)
        if (task.Project.UserId != userId.Value)
        {
            return Result<TaskDetailDto>.Failure("You do not have permission to view this task");
        }

        // Get dependencies
        var dependencyIds = await _context.TaskDependencies
            .Where(td => td.TaskId == task.Id)
            .Select(td => td.DependsOnTaskId)
            .ToListAsync(cancellationToken);

        // Map to DTO
        var taskDetail = new TaskDetailDto
        {
            Id = task.Id,
            Description = task.Description,
            IsCompleted = task.IsCompleted,
            CompletedAt = task.CompletedAt,
            DueDate = task.DueDate,
            ProjectId = task.ProjectId,
            ProjectTitle = task.Project.Title,
            CreatedAt = task.CreatedAt,
            CreatedBy = task.CreatedBy,
            UpdatedAt = task.UpdatedAt,
            UpdatedBy = task.UpdatedBy,
            EstimatedHours = task.EstimatedHours,
            DependencyIds = dependencyIds
        };

        return Result<TaskDetailDto>.Success(taskDetail);
    }
}
