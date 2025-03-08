using PetSchedule.Core.Entities;
using PetSchedule.Core.Interfaces;

namespace PetSchedule.Infrastructure.Service;

public class InMemoryNotificationService : INotificationService
{
    private readonly List<ScheduledEvent> _scheduledEvents = new List<ScheduledEvent>();
    private int _eventIdCounter = 0;

    public void AddScheduledEvent(ScheduledEvent scheduledEvent)
    {
        scheduledEvent.Id = ++_eventIdCounter;
        _scheduledEvents.Add(scheduledEvent);
    }

    public IEnumerable<ScheduledEvent> GetDueEvents(int withinMinutes)
    {
        var now = DateTime.UtcNow;
        var cutoff = now.AddMinutes(withinMinutes);
        return _scheduledEvents
            .Where(e => !e.IsNotified && e.ScheduledTime >= now && e.ScheduledTime <= cutoff)
            .ToList();
    }

    public void MarkAsNotified(int eventId)
    {
        var evt = _scheduledEvents.FirstOrDefault(e => e.Id == eventId);
        if (evt != null)
        {
            evt.IsNotified = true;
        }
    }
}