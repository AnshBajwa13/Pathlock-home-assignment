using Application.Common.Models;
using Application.Tasks.DTOs;
using MediatR;

namespace Application.Tasks.Queries.GetTasksByProject;

/// <summary>
/// Query to get tasks by project ID
/// </summary>
public class GetTasksByProjectQuery : IRequest<Result<List<TaskDto>>>
{
    public Guid ProjectId { get; set; }
    public bool? IsCompleted { get; set; } // Optional filter
}
