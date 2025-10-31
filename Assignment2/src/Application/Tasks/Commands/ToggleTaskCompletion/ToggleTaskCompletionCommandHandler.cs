using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Tasks.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Tasks.Commands.ToggleTaskCompletion;

/// <summary>
/// Handler for toggling task completion status
/// </summary>
public class ToggleTaskCompletionCommandHandler : IRequestHandler<ToggleTaskCompletionCommand, Result<TaskDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ToggleTaskCompletionCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<TaskDto>> Handle(ToggleTaskCompletionCommand request, CancellationToken cancellationToken)
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

        // Toggle completion status
        task.IsCompleted = !task.IsCompleted;
        task.CompletedAt = task.IsCompleted ? DateTime.UtcNow : null;
        task.UpdatedAt = DateTime.UtcNow;
        task.UpdatedBy = userId.Value;

        await _context.SaveChangesAsync(cancellationToken);

        // Return DTO
        var taskDto = new TaskDto
        {
            Id = task.Id,
            Description = task.Description,
            IsCompleted = task.IsCompleted,
            CompletedAt = task.CompletedAt,
            DueDate = task.DueDate,
            ProjectId = task.ProjectId,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };

        return Result<TaskDto>.Success(taskDto);
    }
}
