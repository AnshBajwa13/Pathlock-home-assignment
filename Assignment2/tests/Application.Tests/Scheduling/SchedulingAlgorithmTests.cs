using Application.Scheduling.Services;
using Domain.Entities;
using Xunit;

namespace Application.Tests.Scheduling;

public class SchedulingAlgorithmTests
{
    private readonly SchedulingAlgorithm _algorithm;

    public SchedulingAlgorithmTests()
    {
        _algorithm = new SchedulingAlgorithm();
    }

    [Fact]
    public void GenerateSchedule_EmptyTaskList_ReturnsWarning()
    {
        // Arrange
        var tasks = new List<TaskItem>();

        // Act
        var result = _algorithm.GenerateSchedule(tasks);

        // Assert
        Assert.Single(result.Warnings);
        Assert.Contains("No tasks to schedule", result.Warnings[0]);
    }

    [Fact]
    public void GenerateSchedule_SingleTask_ReturnsCorrectSchedule()
    {
        // Arrange
        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Task 1",
            EstimatedHours = 5,
            ProjectId = Guid.NewGuid()
        };
        var tasks = new List<TaskItem> { task };

        // Act
        var result = _algorithm.GenerateSchedule(tasks);

        // Assert
        Assert.Single(result.OrderedTasks);
        Assert.Equal(task.Id, result.OrderedTasks[0]);
        Assert.Equal(5, result.TotalEstimatedHours);
        Assert.Single(result.CriticalPath);
        Assert.Equal(task.Id, result.CriticalPath[0]);
    }

    [Fact]
    public void GenerateSchedule_LinearDependencyChain_ReturnsCorrectOrder()
    {
        // Arrange
        var task1 = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Task 1",
            EstimatedHours = 2,
            ProjectId = Guid.NewGuid(),
            DependsOn = new List<TaskDependency>()
        };

        var task2 = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Task 2",
            EstimatedHours = 3,
            ProjectId = task1.ProjectId,
            DependsOn = new List<TaskDependency>
            {
                new TaskDependency { TaskId = Guid.NewGuid(), DependsOnTaskId = task1.Id }
            }
        };

        var task3 = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Task 3",
            EstimatedHours = 4,
            ProjectId = task1.ProjectId,
            DependsOn = new List<TaskDependency>
            {
                new TaskDependency { TaskId = Guid.NewGuid(), DependsOnTaskId = task2.Id }
            }
        };

        var tasks = new List<TaskItem> { task3, task1, task2 }; // Intentionally out of order

        // Act
        var result = _algorithm.GenerateSchedule(tasks);

        // Assert
        Assert.Equal(3, result.OrderedTasks.Count);
        Assert.Equal(task1.Id, result.OrderedTasks[0]); // Task1 first
        Assert.Equal(task2.Id, result.OrderedTasks[1]); // Task2 second
        Assert.Equal(task3.Id, result.OrderedTasks[2]); // Task3 last
        Assert.Equal(9, result.TotalEstimatedHours); // 2 + 3 + 4
    }

    [Fact]
    public void GenerateSchedule_CircularDependency_ReturnsWarning()
    {
        // Arrange
        var task1 = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Task 1",
            EstimatedHours = 2,
            ProjectId = Guid.NewGuid()
        };

        var task2 = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Task 2",
            EstimatedHours = 3,
            ProjectId = task1.ProjectId
        };

        // Circular dependency: task1 -> task2 -> task1
        task1.DependsOn = new List<TaskDependency>
        {
            new TaskDependency { TaskId = task1.Id, DependsOnTaskId = task2.Id }
        };
        task2.DependsOn = new List<TaskDependency>
        {
            new TaskDependency { TaskId = task2.Id, DependsOnTaskId = task1.Id }
        };

        var tasks = new List<TaskItem> { task1, task2 };

        // Act
        var result = _algorithm.GenerateSchedule(tasks);

        // Assert
        Assert.Single(result.Warnings);
        Assert.Contains("Circular dependency", result.Warnings[0]);
        Assert.Empty(result.OrderedTasks);
    }

    [Fact]
    public void GenerateSchedule_ParallelTasks_AllOnCriticalPath()
    {
        // Arrange
        var task1 = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Task 1",
            EstimatedHours = 5,
            ProjectId = Guid.NewGuid(),
            DependsOn = new List<TaskDependency>()
        };

        var task2 = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Task 2",
            EstimatedHours = 5,
            ProjectId = task1.ProjectId,
            DependsOn = new List<TaskDependency>()
        };

        var tasks = new List<TaskItem> { task1, task2 };

        // Act
        var result = _algorithm.GenerateSchedule(tasks);

        // Assert
        Assert.Equal(2, result.OrderedTasks.Count);
        Assert.Equal(2, result.CriticalPath.Count); // Both are critical
        Assert.Equal(0, result.Slack[task1.Id]); // No slack
        Assert.Equal(0, result.Slack[task2.Id]); // No slack
    }

    [Fact]
    public void GenerateSchedule_TasksWithoutEstimates_UsesDefaultAndWarns()
    {
        // Arrange
        var task1 = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Task 1",
            EstimatedHours = null, // No estimate
            ProjectId = Guid.NewGuid(),
            DependsOn = new List<TaskDependency>()
        };

        var tasks = new List<TaskItem> { task1 };

        // Act
        var result = _algorithm.GenerateSchedule(tasks);

        // Assert
        Assert.Single(result.OrderedTasks);
        Assert.Contains("no estimated hours", result.Warnings.FirstOrDefault() ?? "");
    }

    [Fact]
    public void GenerateSchedule_ComplexDependencyGraph_CalculatesCorrectCPM()
    {
        // Arrange - Diamond dependency pattern
        //     Task1 (3h)
        //    /         \
        // Task2 (2h)   Task3 (5h)  <- Task3 is longer, so critical path
        //    \         /
        //     Task4 (1h)
        
        var task1 = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Task 1",
            EstimatedHours = 3,
            ProjectId = Guid.NewGuid(),
            DependsOn = new List<TaskDependency>()
        };

        var task2 = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Task 2",
            EstimatedHours = 2,
            ProjectId = task1.ProjectId,
            DependsOn = new List<TaskDependency>
            {
                new TaskDependency { TaskId = Guid.NewGuid(), DependsOnTaskId = task1.Id }
            }
        };

        var task3 = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Task 3",
            EstimatedHours = 5,
            ProjectId = task1.ProjectId,
            DependsOn = new List<TaskDependency>
            {
                new TaskDependency { TaskId = Guid.NewGuid(), DependsOnTaskId = task1.Id }
            }
        };

        var task4 = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Task 4",
            EstimatedHours = 1,
            ProjectId = task1.ProjectId,
            DependsOn = new List<TaskDependency>
            {
                new TaskDependency { TaskId = Guid.NewGuid(), DependsOnTaskId = task2.Id },
                new TaskDependency { TaskId = Guid.NewGuid(), DependsOnTaskId = task3.Id }
            }
        };

        var tasks = new List<TaskItem> { task4, task2, task3, task1 };

        // Act
        var result = _algorithm.GenerateSchedule(tasks);

        // Assert
        Assert.Equal(4, result.OrderedTasks.Count);
        
        // Critical path should be Task1 -> Task3 -> Task4 (total: 3 + 5 + 1 = 9 hours)
        Assert.Equal(9, result.TotalEstimatedHours);
        Assert.Contains(task1.Id, result.CriticalPath);
        Assert.Contains(task3.Id, result.CriticalPath);
        Assert.Contains(task4.Id, result.CriticalPath);
        
        // Task2 should have slack since it's on the shorter path
        Assert.True(result.Slack[task2.Id] > 0);
        
        // Critical path tasks should have zero slack
        Assert.Equal(0, result.Slack[task1.Id]);
        Assert.Equal(0, result.Slack[task3.Id]);
        Assert.Equal(0, result.Slack[task4.Id]);
    }

    [Fact]
    public void GenerateSchedule_OverdueTasks_ReturnsWarning()
    {
        // Arrange
        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Overdue Task",
            EstimatedHours = 5,
            ProjectId = Guid.NewGuid(),
            DueDate = DateTime.UtcNow.AddDays(-1), // Yesterday
            DependsOn = new List<TaskDependency>()
        };

        var tasks = new List<TaskItem> { task };

        // Act
        var result = _algorithm.GenerateSchedule(tasks);

        // Assert
        Assert.Contains(result.Warnings, w => w.Contains("past due dates"));
    }
}
