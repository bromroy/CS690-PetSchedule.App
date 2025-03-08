using PetSchedule.Core.Entities;

namespace PetSchedule.Core.Interfaces; 

public interface INotificationService
{
    void AddScheduledEvent(ScheduledEvent scheduledEvent);
    IEnumerable<ScheduledEvent> GetDueEvents(int withinMinutes);
    void MarkAsNotified(int eventId);
}