using Application.Common.Models;
using Application.Projects.DTOs;
using MediatR;

namespace Application.Projects.Queries.GetProjects;

/// <summary>
/// Query to get all projects for the current user with pagination
/// </summary>
public class GetProjectsQuery : IRequest<Result<PaginatedList<ProjectDto>>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
