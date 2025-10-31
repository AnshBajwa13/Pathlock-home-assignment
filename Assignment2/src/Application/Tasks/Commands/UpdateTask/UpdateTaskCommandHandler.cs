using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Tasks.DTOs;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Tasks.Commands.UpdateTask;

/// <summary>
/// Handler for updating a task
/// </summary>
public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, Result<TaskDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateTaskCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<TaskDto>> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        // Get current user ID
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            return Result<TaskDto>.Failure("User not authenticated");
        }

        // Find task with project
        var task = await _context.Tasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (task == null)
        {
            return Result<TaskDto>.Failure("Task not found");
        }

        // Check ownership (user must own the project)
        if (task.Project.UserId != userId.Value)
        {
            return Result<TaskDto>.Failure("You do not have permission to update this task");
        }

        // Update task
        task.Title = request.Title;
        task.Description = request.Description;
        task.DueDate = request.DueDate;
        task.EstimatedHours = request.EstimatedHours;
        task.UpdatedAt = DateTime.UtcNow;
        task.UpdatedBy = userId.Value;

        // Update dependencies
        // Remove existing dependencies
        var existingDeps = _context.TaskDependencies.Where(d => d.TaskId == task.Id);
        _context.TaskDependencies.RemoveRange(existingDeps);

        // Add new dependencies
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
        }

        await _context.SaveChangesAsync(cancellationToken);

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
