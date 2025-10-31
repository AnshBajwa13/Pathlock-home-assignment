using Domain.Entities;

namespace Application.Scheduling.Services;

public interface ISchedulingAlgorithm
{
    SchedulingResult GenerateSchedule(List<TaskItem> tasks);
}

public class SchedulingAlgorithm : ISchedulingAlgorithm
{
    public SchedulingResult GenerateSchedule(List<TaskItem> tasks)
    {
        var result = new SchedulingResult();

        if (tasks.Count == 0)
        {
            result.Warnings.Add("No tasks to schedule.");
            return result;
        }

        // Build dependency graph
        var taskMap = tasks.ToDictionary(t => t.Id);
        var inDegree = tasks.ToDictionary(t => t.Id, t => 0);
        var adjacencyList = tasks.ToDictionary(t => t.Id, t => new List<Guid>());

        foreach (var task in tasks)
        {
            if (task.DependsOn != null)
            {
                foreach (var dep in task.DependsOn)
                {
                    if (taskMap.ContainsKey(dep.DependsOnTaskId))
                    {
                        adjacencyList[dep.DependsOnTaskId].Add(task.Id);
                        inDegree[task.Id]++;
                    }
                }
            }
        }

        // Topological Sort using Kahn's Algorithm
        var topologicalOrder = TopologicalSort(tasks, inDegree, adjacencyList);

        if (topologicalOrder == null)
        {
            result.Warnings.Add("Circular dependency detected! Cannot generate valid schedule.");
            return result;
        }

        // Critical Path Method (CPM)
        var cpmResult = CalculateCriticalPath(tasks, topologicalOrder, taskMap, adjacencyList);
        
        result.OrderedTasks = topologicalOrder;
        result.CriticalPath = cpmResult.CriticalPath;
        result.EarliestStart = cpmResult.EarliestStart;
        result.EarliestFinish = cpmResult.EarliestFinish;
        result.LatestStart = cpmResult.LatestStart;
        result.LatestFinish = cpmResult.LatestFinish;
        result.Slack = cpmResult.Slack;
        result.TotalEstimatedHours = cpmResult.TotalCriticalPathHours;

        // Add warnings for tasks without estimates
        var tasksWithoutEstimates = tasks.Where(t => !t.EstimatedHours.HasValue).ToList();
        if (tasksWithoutEstimates.Any())
        {
            result.Warnings.Add($"{tasksWithoutEstimates.Count} task(s) have no estimated hours. Assuming 1 hour for scheduling.");
        }

        // Warn about tasks with past due dates
        var overdueTasks = tasks.Where(t => t.DueDate.HasValue && t.DueDate < DateTime.UtcNow).ToList();
        if (overdueTasks.Any())
        {
            result.Warnings.Add($"{overdueTasks.Count} task(s) have past due dates.");
        }

        return result;
    }

    private List<Guid>? TopologicalSort(
        List<TaskItem> tasks,
        Dictionary<Guid, int> inDegree,
        Dictionary<Guid, List<Guid>> adjacencyList)
    {
        var queue = new Queue<Guid>();
        var result = new List<Guid>();

        // Start with tasks that have no dependencies
        foreach (var task in tasks)
        {
            if (inDegree[task.Id] == 0)
            {
                queue.Enqueue(task.Id);
            }
        }

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            result.Add(current);

            foreach (var neighbor in adjacencyList[current])
            {
                inDegree[neighbor]--;
                if (inDegree[neighbor] == 0)
                {
                    queue.Enqueue(neighbor);
                }
            }
        }

        // If we haven't processed all tasks, there's a cycle
        if (result.Count != tasks.Count)
        {
            return null;
        }

        return result;
    }

    private CpmResult CalculateCriticalPath(
        List<TaskItem> tasks,
        List<Guid> topologicalOrder,
        Dictionary<Guid, TaskItem> taskMap,
        Dictionary<Guid, List<Guid>> adjacencyList)
    {
        var result = new CpmResult();
        
        // Default estimate for tasks without hours
        decimal GetEstimate(TaskItem task) => task.EstimatedHours ?? 1m;

        // Forward pass - Calculate Earliest Start and Earliest Finish
        foreach (var taskId in topologicalOrder)
        {
            var task = taskMap[taskId];
            var estimate = GetEstimate(task);

            // Find maximum earliest finish of all predecessors
            decimal maxPredecessorFinish = 0m;
            if (task.DependsOn != null && task.DependsOn.Any())
            {
                foreach (var dep in task.DependsOn)
                {
                    if (result.EarliestFinish.ContainsKey(dep.DependsOnTaskId))
                    {
                        maxPredecessorFinish = Math.Max(
                            maxPredecessorFinish,
                            result.EarliestFinish[dep.DependsOnTaskId]
                        );
                    }
                }
            }

            result.EarliestStart[taskId] = maxPredecessorFinish;
            result.EarliestFinish[taskId] = maxPredecessorFinish + estimate;
        }

        // Project completion time
        decimal projectCompletionTime = result.EarliestFinish.Values.Max();

        // Backward pass - Calculate Latest Start and Latest Finish
        for (int i = topologicalOrder.Count - 1; i >= 0; i--)
        {
            var taskId = topologicalOrder[i];
            var task = taskMap[taskId];
            var estimate = GetEstimate(task);

            // Find minimum latest start of all successors
            decimal minSuccessorStart = projectCompletionTime;
            var successors = adjacencyList[taskId];
            
            if (successors.Any())
            {
                foreach (var successorId in successors)
                {
                    if (result.LatestStart.ContainsKey(successorId))
                    {
                        minSuccessorStart = Math.Min(
                            minSuccessorStart,
                            result.LatestStart[successorId]
                        );
                    }
                }
            }

            result.LatestFinish[taskId] = minSuccessorStart;
            result.LatestStart[taskId] = minSuccessorStart - estimate;
        }

        // Calculate slack and identify critical path
        foreach (var taskId in topologicalOrder)
        {
            decimal slack = result.LatestStart[taskId] - result.EarliestStart[taskId];
            result.Slack[taskId] = slack;

            if (slack == 0)
            {
                result.CriticalPath.Add(taskId);
            }
        }

        result.TotalCriticalPathHours = result.CriticalPath
            .Sum(id => GetEstimate(taskMap[id]));

        return result;
    }
}

public class SchedulingResult
{
    public List<Guid> OrderedTasks { get; set; } = new();
    public List<Guid> CriticalPath { get; set; } = new();
    public Dictionary<Guid, decimal> EarliestStart { get; set; } = new();
    public Dictionary<Guid, decimal> EarliestFinish { get; set; } = new();
    public Dictionary<Guid, decimal> LatestStart { get; set; } = new();
    public Dictionary<Guid, decimal> LatestFinish { get; set; } = new();
    public Dictionary<Guid, decimal> Slack { get; set; } = new();
    public decimal TotalEstimatedHours { get; set; }
    public List<string> Warnings { get; set; } = new();
}

public class CpmResult
{
    public List<Guid> CriticalPath { get; set; } = new();
    public Dictionary<Guid, decimal> EarliestStart { get; set; } = new();
    public Dictionary<Guid, decimal> EarliestFinish { get; set; } = new();
    public Dictionary<Guid, decimal> LatestStart { get; set; } = new();
    public Dictionary<Guid, decimal> LatestFinish { get; set; } = new();
    public Dictionary<Guid, decimal> Slack { get; set; } = new();
    public decimal TotalCriticalPathHours { get; set; }
}
