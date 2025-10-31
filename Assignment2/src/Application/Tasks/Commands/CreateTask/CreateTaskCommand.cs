using Application.Common.Models;
using Application.Tasks.DTOs;
using MediatR;

namespace Application.Tasks.Commands.CreateTask;

/// <summary>
/// Command to create a new task
/// </summary>
public class CreateTaskCommand : IRequest<Result<TaskDto>>
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal? EstimatedHours { get; set; }
    public List<Guid> DependencyIds { get; set; } = new();
    public Guid ProjectId { get; set; }
}
