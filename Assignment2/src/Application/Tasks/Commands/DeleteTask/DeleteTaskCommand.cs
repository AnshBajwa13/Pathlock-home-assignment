using Application.Common.Models;
using MediatR;

namespace Application.Tasks.Commands.DeleteTask;

/// <summary>
/// Command to delete a task (soft delete)
/// </summary>
public class DeleteTaskCommand : IRequest<Result<bool>>
{
    public Guid Id { get; set; }
}
