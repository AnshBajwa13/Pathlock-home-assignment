using Application.Common.Models;
using MediatR;

namespace Application.Projects.Commands.DeleteProject;

/// <summary>
/// Command to soft delete a project
/// </summary>
public class DeleteProjectCommand : IRequest<Result>
{
    public Guid Id { get; set; }
}
