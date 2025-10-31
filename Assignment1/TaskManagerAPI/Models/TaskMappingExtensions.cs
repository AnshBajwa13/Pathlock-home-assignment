using TaskManagerAPI.Models.Domain;
using TaskManagerAPI.Models.DTOs;

namespace TaskManagerAPI.Models;

/// <summary>
/// Extension methods for mapping between domain models and DTOs
/// This keeps controllers and services clean
/// </summary>
public static class TaskMappingExtensions
{
    /// <summary>
    /// Convert a TaskItem domain model to a TaskResponse DTO
    /// </summary>
    public static TaskResponse ToResponse(this TaskItem task)
    {
        return new TaskResponse
        {
            Id = task.Id,
            Description = task.Description,
            IsCompleted = task.IsCompleted,
            CreatedAt = task.CreatedAt,
            CompletedAt = task.CompletedAt
        };
    }

    /// <summary>
    /// Convert a list of TaskItem domain models to TaskResponse DTOs
    /// </summary>
    public static IEnumerable<TaskResponse> ToResponses(this IEnumerable<TaskItem> tasks)
    {
        return tasks.Select(ToResponse);
    }
}
