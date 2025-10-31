using MediatR;

namespace Application.Scheduling.Commands.GenerateSchedule;

public class GenerateScheduleCommand : IRequest<ScheduleDto>
{
    public Guid ProjectId { get; set; }
}
