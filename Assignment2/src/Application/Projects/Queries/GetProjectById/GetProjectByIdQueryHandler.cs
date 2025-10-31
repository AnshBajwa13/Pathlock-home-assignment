using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Projects.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Projects.Queries.GetProjectById;

/// <summary>
/// Handler for getting a project by ID
/// </summary>
public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, Result<ProjectDetailDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetProjectByIdQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<ProjectDetailDto>> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        // Get current user ID
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            return Result<ProjectDetailDto>.Failure("User not authenticated");
        }

        // Find project with tasks
        var project = await _context.Projects
            .Include(p => p.Tasks.Where(t => !t.IsDeleted))
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (project == null)
        {
            return Result<ProjectDetailDto>.Failure("Project not found");
        }

        // Check ownership
        if (project.UserId != userId.Value)
        {
            return Result<ProjectDetailDto>.Failure("You do not have permission to view this project");
        }

        // Map to DTO
        var projectDetail = new ProjectDetailDto
        {
            Id = project.Id,
            Title = project.Title,
            Description = project.Description,
            UserId = project.UserId,
            CreatedAt = project.CreatedAt,
            CreatedBy = project.CreatedBy,
            UpdatedAt = project.UpdatedAt,
            UpdatedBy = project.UpdatedBy,
            Tasks = project.Tasks
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new TaskSummaryDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    IsCompleted = t.IsCompleted,
                    CompletedAt = t.CompletedAt,
                    CreatedAt = t.CreatedAt
                })
                .ToList()
        };

        return Result<ProjectDetailDto>.Success(projectDetail);
    }
}
