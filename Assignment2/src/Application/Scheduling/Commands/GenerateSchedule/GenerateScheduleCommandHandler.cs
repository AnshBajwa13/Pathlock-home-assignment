using Application.Common.Interfaces;
using Application.Scheduling.Services;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Scheduling.Commands.GenerateSchedule;

public class GenerateScheduleCommandHandler : IRequestHandler<GenerateScheduleCommand, ScheduleDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ISchedulingAlgorithm _schedulingAlgorithm;

    public GenerateScheduleCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        ISchedulingAlgorithm schedulingAlgorithm)
    {
        _context = context;
        _currentUserService = currentUserService;
        _schedulingAlgorithm = schedulingAlgorithm;
    }

    public async Task<ScheduleDto> Handle(GenerateScheduleCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId 
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        // Verify project exists and belongs to user
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == request.ProjectId && p.UserId == userId, cancellationToken);

        if (project == null)
        {
            throw new KeyNotFoundException($"Project with ID {request.ProjectId} not found or access denied.");
        }

        // Load all tasks for the project with their dependencies
        var tasks = await _context.Tasks
            .Include(t => t.DependsOn)
            .Where(t => t.ProjectId == request.ProjectId)
            .ToListAsync(cancellationToken);

        // Generate schedule using the algorithm
        var schedulingResult = _schedulingAlgorithm.GenerateSchedule(tasks);

        // Build the response DTO
        var scheduleDto = new ScheduleDto
        {
            CriticalPath = schedulingResult.CriticalPath,
            TotalEstimatedHours = schedulingResult.TotalEstimatedHours,
            Warnings = schedulingResult.Warnings,
            ScheduledTasks = new List<ScheduledTaskDto>()
        };

        // Map tasks to DTOs with scheduling information
        var taskMap = tasks.ToDictionary(t => t.Id);
        for (int i = 0; i < schedulingResult.OrderedTasks.Count; i++)
        {
            var taskId = schedulingResult.OrderedTasks[i];
            var task = taskMap[taskId];

            var scheduledTask = new ScheduledTaskDto
            {
                TaskId = task.Id,
                Title = task.Title,
                Order = i + 1,
                EstimatedHours = task.EstimatedHours,
                EarliestStart = schedulingResult.EarliestStart[taskId],
                EarliestFinish = schedulingResult.EarliestFinish[taskId],
                LatestStart = schedulingResult.LatestStart[taskId],
                LatestFinish = schedulingResult.LatestFinish[taskId],
                Slack = schedulingResult.Slack[taskId],
                IsCritical = schedulingResult.CriticalPath.Contains(taskId),
                Dependencies = task.DependsOn?.Select(d => d.DependsOnTaskId).ToList() ?? new List<Guid>(),
                DueDate = task.DueDate
            };

            scheduleDto.ScheduledTasks.Add(scheduledTask);
        }

        return scheduleDto;
    }
}
