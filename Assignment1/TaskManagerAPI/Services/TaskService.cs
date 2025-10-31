using TaskManagerAPI.Models;
using TaskManagerAPI.Models.Domain;
using TaskManagerAPI.Models.DTOs;
using TaskManagerAPI.Repositories;

namespace TaskManagerAPI.Services;

/// <summary>
/// Implementation of ITaskService.
/// Contains business logic for task operations.
/// This is the clean, simple Service Layer approach - NO CQRS/MediatR overhead.
/// </summary>
public class TaskService : ITaskService
{
    private readonly ITaskRepository _repository;
    private readonly ILogger<TaskService> _logger;

    public TaskService(ITaskRepository repository, ILogger<TaskService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<TaskResponse>> GetAllAsync()
    {
        _logger.LogInformation("Fetching all tasks");
        
        var tasks = await _repository.GetAllAsync();
        return tasks.ToResponses();
    }

    public async Task<TaskResponse?> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Fetching task with ID: {TaskId}", id);
        
        var task = await _repository.GetByIdAsync(id);
        return task?.ToResponse();
    }

    public async Task<TaskResponse> CreateAsync(CreateTaskRequest request)
    {
        _logger.LogInformation("Creating new task with description: {Description}", request.Description);
        
        // Domain model handles validation through constructor
        var task = new TaskItem(request.Description);
        
        await _repository.AddAsync(task);
        
        _logger.LogInformation("Task created successfully with ID: {TaskId}", task.Id);
        
        return task.ToResponse();
    }

    public async Task<TaskResponse> UpdateAsync(Guid id, UpdateTaskRequest request)
    {
        _logger.LogInformation("Updating task with ID: {TaskId}", id);
        
        var task = await _repository.GetByIdAsync(id);
        
        if (task == null)
        {
            _logger.LogWarning("Task not found with ID: {TaskId}", id);
            throw new KeyNotFoundException($"Task with ID {id} not found");
        }

        // Update description if changed
        if (task.Description != request.Description)
        {
            task.UpdateDescription(request.Description);
        }

        // Update completion status if changed
        if (task.IsCompleted != request.IsCompleted)
        {
            if (request.IsCompleted)
                task.MarkAsCompleted();
            else
                task.MarkAsIncomplete();
        }

        await _repository.UpdateAsync(task);
        
        _logger.LogInformation("Task updated successfully: {TaskId}", task.Id);
        
        return task.ToResponse();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        _logger.LogInformation("Deleting task with ID: {TaskId}", id);
        
        var exists = await _repository.ExistsAsync(id);
        
        if (!exists)
        {
            _logger.LogWarning("Task not found for deletion: {TaskId}", id);
            return false;
        }

        await _repository.DeleteAsync(id);
        
        _logger.LogInformation("Task deleted successfully: {TaskId}", id);
        
        return true;
    }

    public async Task<TaskResponse> ToggleCompletionAsync(Guid id)
    {
        _logger.LogInformation("Toggling completion for task: {TaskId}", id);
        
        var task = await _repository.GetByIdAsync(id);
        
        if (task == null)
        {
            _logger.LogWarning("Task not found with ID: {TaskId}", id);
            throw new KeyNotFoundException($"Task with ID {id} not found");
        }

        task.ToggleCompletion();
        await _repository.UpdateAsync(task);
        
        _logger.LogInformation("Task completion toggled: {TaskId}, IsCompleted: {IsCompleted}", 
            task.Id, task.IsCompleted);
        
        return task.ToResponse();
    }
}
