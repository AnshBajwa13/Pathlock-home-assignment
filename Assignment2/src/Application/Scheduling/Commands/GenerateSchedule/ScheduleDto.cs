namespace Application.Scheduling.Commands.GenerateSchedule;

public class ScheduleDto
{
    public List<ScheduledTaskDto> ScheduledTasks { get; set; } = new();
    public List<Guid> CriticalPath { get; set; } = new();
    public decimal TotalEstimatedHours { get; set; }
    public List<string> Warnings { get; set; } = new();
}

public class ScheduledTaskDto
{
    public Guid TaskId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Order { get; set; }
    public decimal? EstimatedHours { get; set; }
    public decimal EarliestStart { get; set; }
    public decimal LatestStart { get; set; }
    public decimal EarliestFinish { get; set; }
    public decimal LatestFinish { get; set; }
    public decimal Slack { get; set; }
    public bool IsCritical { get; set; }
    public List<Guid> Dependencies { get; set; } = new();
    public DateTime? DueDate { get; set; }
}
