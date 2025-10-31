using Application.Common.Models;
using Application.Projects.DTOs;
using MediatR;

namespace Application.Projects.Queries.GetProjectById;

/// <summary>
/// Query to get a project by ID with full details
/// </summary>
public class GetProjectByIdQuery : IRequest<Result<ProjectDetailDto>>
{
    public Guid Id { get; set; }
}
