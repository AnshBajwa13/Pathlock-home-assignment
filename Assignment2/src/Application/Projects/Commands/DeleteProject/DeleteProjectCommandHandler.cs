using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Projects.Commands.DeleteProject;

/// <summary>
/// Handler for soft deleting a project
/// </summary>
public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DeleteProjectCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        // Get current user ID
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            return Result.Failure("User not authenticated");
        }

        // Find project
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (project == null)
        {
            return Result.Failure("Project not found");
        }

        // Check ownership
        if (project.UserId != userId.Value)
        {
            return Result.Failure("You do not have permission to delete this project");
        }

        // Soft delete (mark as deleted)
        project.IsDeleted = true;
        project.DeletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
