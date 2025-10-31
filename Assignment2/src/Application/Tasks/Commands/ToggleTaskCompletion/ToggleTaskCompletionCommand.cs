using Application.Common.Models;
using Application.Tasks.DTOs;
using MediatR;

namespace Application.Tasks.Commands.ToggleTaskCompletion;

/// <summary>
/// Command to toggle task completion status
/// </summary>
public class ToggleTaskCompletionCommand : IRequest<Result<TaskDto>>
{
    public Guid Id { get; set; }
}
