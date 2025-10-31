using Application.Common.Models;
using Application.Projects.DTOs;
using MediatR;

namespace Application.Projects.Commands.UpdateProject;

/// <summary>
/// Command to update an existing project
/// </summary>
public class UpdateProjectCommand : IRequest<Result<ProjectDto>>
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
