using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Tasks.DTOs;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Tasks.Commands.CreateTask;

/// <summary>
/// Handler for creating a new task
/// </summary>
public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Result<TaskDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateTaskCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<TaskDto>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        // Get current user ID
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            return Result<TaskDto>.Failure("User not authenticated");
        }

        // Verify project exists and user owns it
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken);

        if (project == null)
        {
            return Result<TaskDto>.Failure("Project not found");
        }

        if (project.UserId != userId.Value)
        {
            return Result<TaskDto>.Failure("You do not have permission to add tasks to this project");
        }

        // Create task
        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            DueDate = request.DueDate,
            EstimatedHours = request.EstimatedHours,
            ProjectId = request.ProjectId,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId.Value
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync(cancellationToken);

        // Add dependencies if provided
        if (request.DependencyIds != null && request.DependencyIds.Any())
        {
            foreach (var dependencyId in request.DependencyIds)
            {
                var dependency = new TaskDependency
                {
                    TaskId = task.Id,
                    DependsOnTaskId = dependencyId
                };
                _context.TaskDependencies.Add(dependency);
            }
            await _context.SaveChangesAsync(cancellationToken);
        }

        // Return DTO
        var taskDto = new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            IsCompleted = task.IsCompleted,
            CompletedAt = task.CompletedAt,
            DueDate = task.DueDate,
            EstimatedHours = task.EstimatedHours,
            ProjectId = task.ProjectId,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };

        return Result<TaskDto>.Success(taskDto);
    }
}
