using Application.Common.Models;
using Application.Tasks.DTOs;
using MediatR;

namespace Application.Tasks.Queries.GetTaskById;

/// <summary>
/// Query to get a task by ID
/// </summary>
public class GetTaskByIdQuery : IRequest<Result<TaskDetailDto>>
{
    public Guid Id { get; set; }
}
