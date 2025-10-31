using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Projects.DTOs;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Projects.Commands.UpdateProject;

/// <summary>
/// Handler for updating a project
/// </summary>
public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, Result<ProjectDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateProjectCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<ProjectDto>> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        // Get current user ID
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            return Result<ProjectDto>.Failure("User not authenticated");
        }

        // Find project
        var project = await _context.Projects
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (project == null)
        {
            return Result<ProjectDto>.Failure("Project not found");
        }

        // Check ownership
        if (project.UserId != userId.Value)
        {
            return Result<ProjectDto>.Failure("You do not have permission to update this project");
        }

        // Update project
        project.Title = request.Title;
        project.Description = request.Description;
        project.UpdatedAt = DateTime.UtcNow;
        project.UpdatedBy = userId.Value;

        await _context.SaveChangesAsync(cancellationToken);

        // Return DTO
        var projectDto = new ProjectDto
        {
            Id = project.Id,
            Title = project.Title,
            Description = project.Description,
            TaskCount = project.Tasks.Count,
            CompletedTaskCount = project.Tasks.Count(t => t.IsCompleted),
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt
        };

        return Result<ProjectDto>.Success(projectDto);
    }
}
