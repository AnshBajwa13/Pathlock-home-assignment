using Application.Common.Models;
using Application.Projects.DTOs;
using MediatR;

namespace Application.Projects.Commands.CreateProject;

/// <summary>
/// Command to create a new project
/// </summary>
public class CreateProjectCommand : IRequest<Result<ProjectDto>>
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
