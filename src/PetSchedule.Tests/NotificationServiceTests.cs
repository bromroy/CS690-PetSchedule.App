using PetSchedule.Core.Entities;
using PetSchedule.Core.Interfaces;
using PetSchedule.Infrastructure.Service;

namespace PetSchedule.Tests;

public class NotificationServiceTests
{
    private INotificationService _notificationService;

    public NotificationServiceTests()
    {
        _notificationService = new InMemoryNotificationService();
    }

    [Fact]
    public void Should_Add_Scheduled_Event()
    {
        // Arrange
        var evt = new ScheduledEvent
        {
            PetId = 1,
            Type = EventType.Feed,
            ScheduledTime = DateTime.UtcNow.AddMinutes(10)
        };

        // Act
        _notificationService.AddScheduledEvent(evt);

        // Assert
        var dueEvents = _notificationService.GetDueEvents(9999);
        Assert.Single(dueEvents);
        Assert.Equal(EventType.Feed, dueEvents.First().Type);
    }

    [Fact]
    public void Should_Return_No_Events_When_None_Found_Within_Time()
    {
        // Arrange
        // Add an event in 60 minutes
        _notificationService.AddScheduledEvent(new ScheduledEvent
        {
            PetId = 2,
            Type = EventType.Walk,
            ScheduledTime = DateTime.UtcNow.AddMinutes(60)
        });

        // Act
        var dueSoon = _notificationService.GetDueEvents(5);

        // Assert
        Assert.Empty(dueSoon);  // none within 5 minutes
    }

    [Fact]
    public void Should_Return_Events_Due_Within_X_Minutes()
    {
        // Arrange
        var evt1 = new ScheduledEvent
        {
            PetId = 1,
            Type = EventType.Feed,
            ScheduledTime = DateTime.UtcNow.AddMinutes(2)
        };
        var evt2 = new ScheduledEvent
        {
            PetId = 1,
            Type = EventType.Walk,
            ScheduledTime = DateTime.UtcNow.AddMinutes(30)
        };
        _notificationService.AddScheduledEvent(evt1);
        _notificationService.AddScheduledEvent(evt2);

        // Act
        var dueSoon = _notificationService.GetDueEvents(5);

        // Assert
        Assert.Single(dueSoon);
        Assert.Equal(EventType.Feed, dueSoon.First().Type);
    }

    [Fact]
    public void Should_Mark_Event_As_Notified()
    {
        // Arrange
        var evt = new ScheduledEvent
        {
            PetId = 2,
            Type = EventType.Walk,
            ScheduledTime = DateTime.UtcNow.AddMinutes(3)
        };
        _notificationService.AddScheduledEvent(evt);

        // Act
        var dueEvents = _notificationService.GetDueEvents(5);
        Assert.Single(dueEvents);  // event is due soon

        _notificationService.MarkAsNotified(dueEvents.First().Id);

        var after = _notificationService.GetDueEvents(5);

        // Assert
        Assert.Empty(after);
    }

    [Fact]
    public void MarkAsNotified_Should_Not_Throw_When_Event_Not_Found()
    {
        // Arrange
        // no events added
        // Act
        _notificationService.MarkAsNotified(9999);

        // If it doesn't throw, we pass.
        // We can also confirm no coverage missed:
        var stillNone = _notificationService.GetDueEvents(9999);
        Assert.Empty(stillNone);
    }
}
