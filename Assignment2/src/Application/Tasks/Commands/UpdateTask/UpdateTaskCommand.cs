using Application.Common.Models;
using Application.Tasks.DTOs;
using MediatR;

namespace Application.Tasks.Commands.UpdateTask;

/// <summary>
/// Command to update a task
/// </summary>
public class UpdateTaskCommand : IRequest<Result<TaskDto>>
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal? EstimatedHours { get; set; }
    public List<Guid> DependencyIds { get; set; } = new();
}
