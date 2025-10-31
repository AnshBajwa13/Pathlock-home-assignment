using System.Collections.Concurrent;
using TaskManagerAPI.Models.Domain;

namespace TaskManagerAPI.Repositories;

/// <summary>
/// In-memory implementation of ITaskRepository using ConcurrentDictionary for thread-safety.
/// This satisfies the assignment requirement of "in-memory data storage (no database required)".
/// Using ConcurrentDictionary ensures thread-safe operations in a multi-threaded web API environment.
/// </summary>
public class InMemoryTaskRepository : ITaskRepository
{
    // Thread-safe dictionary for concurrent access
    private readonly ConcurrentDictionary<Guid, TaskItem> _tasks = new();

    public Task<IEnumerable<TaskItem>> GetAllAsync()
    {
        // Return all tasks ordered by creation date (newest first)
        var tasks = _tasks.Values.OrderByDescending(t => t.CreatedAt).AsEnumerable();
        return Task.FromResult(tasks);
    }

    public Task<TaskItem?> GetByIdAsync(Guid id)
    {
        _tasks.TryGetValue(id, out var task);
        return Task.FromResult(task);
    }

    public Task AddAsync(TaskItem task)
    {
        if (!_tasks.TryAdd(task.Id, task))
        {
            throw new InvalidOperationException($"Task with ID {task.Id} already exists");
        }

        return Task.CompletedTask;
    }

    public Task UpdateAsync(TaskItem task)
    {
        if (!_tasks.ContainsKey(task.Id))
        {
            throw new InvalidOperationException($"Task with ID {task.Id} does not exist");
        }

        _tasks[task.Id] = task;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id)
    {
        _tasks.TryRemove(id, out _);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(Guid id)
    {
        return Task.FromResult(_tasks.ContainsKey(id));
    }
}
