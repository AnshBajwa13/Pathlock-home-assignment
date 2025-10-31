using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Projects.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Projects.Commands.CreateProject;

/// <summary>
/// Handler for creating a new project
/// </summary>
public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, Result<ProjectDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateProjectCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<ProjectDto>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        // Get current user ID
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            return Result<ProjectDto>.Failure("User not authenticated");
        }

        // Create project
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            UserId = userId.Value,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId.Value
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync(cancellationToken);

        // Return DTO
        var projectDto = new ProjectDto
        {
            Id = project.Id,
            Title = project.Title,
            Description = project.Description,
            TaskCount = 0,
            CompletedTaskCount = 0,
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt
        };

        return Result<ProjectDto>.Success(projectDto);
    }
}
