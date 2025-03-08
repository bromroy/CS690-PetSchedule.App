namespace PetSchedule.Core.Entities;

public class ScheduledEvent
{
    public int Id { get; set; }
    public int PetId { get; set; }
    public EventType Type { get; set; }
    public DateTime ScheduledTime { get; set; }
    public bool IsNotified { get; set; } = false;
}

public enum EventType
{
    Feed,
    Walk
}
