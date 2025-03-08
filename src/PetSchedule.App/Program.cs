using PetSchedule.Core.Entities;
using PetSchedule.Core.Interfaces;
using PetSchedule.Infrastructure.Service;
using Spectre.Console;

namespace PetSchedule.App;

public class Program
{
    // For simplicity, we’ll store references to our services as static fields here:
    private static IPetService _petService = new InMemoryPetService();
    private static INotificationService _notificationService = new InMemoryNotificationService();
    private static IInventoryService _inventoryService = new InMemoryInventoryService();

    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        //Console.WriteLine("Welcome to the PetSchedule Console App (Production-Style)!");
        SeedData(); // Optional: seed some initial data

        // If you want a background timer for notifications, you could do it here.
        // For now, we’ll rely on a user menu to manually check notifications.
        bool running = true;
        while (running)
        {
            // Clear the screen before rendering the menu
         
            running = HandleMainMenu();

            // PrintMenu();
            // var choice = Console.ReadLine();
            // if (!HandleMenuChoice(choice))
            // {
            //     break; // exit the loop => exit the app
            // }
        }
        AnsiConsole.MarkupLine("[bold blue]Exiting PetSchedule App... Goodbye![/]");
        //Console.WriteLine("Exiting PetSchedule App. Goodbye!");
    }
    private static bool HandleMainMenu()
    {

        // Show a fancy header or banner
        AnsiConsole.Write(
            new FigletText("PetSchedule")
                .Color(Color.Green));

        // Ask the user to pick an option from a selection prompt
        var selection = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[bold yellow]Select an option:[/]")
                .PageSize(8)
                .MoreChoicesText("[grey](Move up and down to reveal more menu items)[/]")
                .AddChoices(new[]
                {
            "List all pets",
            "Add a new pet",
            "Feed a pet",
            "Walk a pet",
            "Check upcoming scheduled events",
            "Schedule a new event",
            "Manage food inventory",
            "Exit"
                }));

        // Based on which string was chosen, we call the appropriate method
        switch (selection)
        {

            case "List all pets":
                ClearAndPrint();
                ListAllPets();
                return true;
            case "Add a new pet":
                ClearAndPrint();
                AddPet();
                return true;
            case "Feed a pet":
                ClearAndPrint();
                FeedPet();
                return true;
            case "Walk a pet":
                ClearAndPrint();
                WalkPet();
                return true;
            case "Check upcoming scheduled events":
                ClearAndPrint();
                CheckUpcomingEvents();
                return true;
            case "Schedule a new event":
                ClearAndPrint();
                ScheduleEvent();
                return true;
            case "Manage food inventory":
                ClearAndPrint();
                ManageInventory();
                return true;
            case "Exit":
                ClearAndPrint();
                return false;
            default:
                return true;
        }
    }

    // clear screen and print output
    static void ClearAndPrint()
    {
        Console.Clear();

    }

    #region Pet Management
    static void ListAllPets()
    {
        var pets = _petService.GetAllPets();
        Console.WriteLine("\n--- PET LIST ---");
        foreach (var pet in pets)
        {
            Console.WriteLine($"ID: {pet.Id}, Name: {pet.Name}");
            // If you want to see feed/walk counts:
            Console.WriteLine($"   FeedRecords: {pet.FeedRecords.Count}, WalkRecords: {pet.WalkRecords.Count}");
        }
    }

    static void AddPet()
    {
        Console.Write("Enter pet name: ");
        string? name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Pet name cannot be empty.");
            return;
        }
        var pet = _petService.AddPet(name);
        Console.WriteLine($"Pet '{pet.Name}' added with ID {pet.Id}.");
    }

    static void FeedPet()
    {
        Console.Write("Enter Pet ID to feed: ");
        if (!int.TryParse(Console.ReadLine(), out int petId))
        {
            Console.WriteLine("Invalid Pet ID.");
            return;
        }

        Console.Write("Enter amount of food (e.g., cups): ");
        if (!double.TryParse(Console.ReadLine(), out double amount))
        {
            Console.WriteLine("Invalid amount.");
            return;
        }

        // Attempt to use from inventory first
        bool success = _inventoryService.UseFoodStock(amount);
        if (!success)
        {
            Console.WriteLine("Not enough food in inventory! Please add more stock first.");
            return;
        }

        var record = _petService.AddFeedRecord(petId, amount);
        var pet = _petService.GetPetById(petId);
        Console.WriteLine($"Pet  {pet!.Name} was fed {amount} units at {record.FeedTime} (Record ID: {record.Id}).");
    }

    static void WalkPet()
    {
        Console.Write("Enter Pet ID to walk: ");
        if (!int.TryParse(Console.ReadLine(), out int petId))
        {
            Console.WriteLine("Invalid Pet ID.");
            return;
        }

        Console.Write("How many minutes for the walk? ");
        if (!int.TryParse(Console.ReadLine(), out int duration))
        {
            Console.WriteLine("Invalid duration.");
            return;
        }

        var start = DateTime.UtcNow;
        var end = start.AddMinutes(duration);

        var record = _petService.AddWalkRecord(petId, start, end);
        var pet = _petService.GetPetById(petId);
        Console.WriteLine($"Pet  {pet!.Name} walked from {record.WalkStart} to {record.WalkEnd} (Record ID: {record.Id}).");
    }
    #endregion

    #region Scheduling/Notifications
    static void CheckUpcomingEvents()
    {
        Console.Write("Check events within how many minutes? ");
        if (!int.TryParse(Console.ReadLine(), out int minutes))
        {
            minutes = 10; // default
        }

        var dueEvents = _notificationService.GetDueEvents(minutes);
        if (!dueEvents.Any())
        {
            Console.WriteLine("No scheduled events within that timeframe.");
            return;
        }

        Console.WriteLine("\n--- UPCOMING EVENTS ---");
        foreach (var evt in dueEvents)
        {
            var pet = _petService.GetPetById(evt.PetId);
            Console.WriteLine($"Event ID: {evt.Id}, Pet: {pet!.Name}, Type: {evt.Type}, Time: {evt.ScheduledTime}");
           
            // Mark as notified so we don't see it again
            _notificationService.MarkAsNotified(evt.Id);
        }
    }

    static void ScheduleEvent()
    {
        Console.Write("Enter Pet ID: ");
        if (!int.TryParse(Console.ReadLine(), out int petId))
        {
            Console.WriteLine("Invalid Pet ID.");
            return;
        }

        Console.Write("Event type (1=Feed, 2=Walk): ");
        var typeInput = Console.ReadLine();
        EventType evtType = (typeInput == "2") ? EventType.Walk : EventType.Feed;

        Console.Write("How many minutes from now to schedule? ");
        if (!int.TryParse(Console.ReadLine(), out int offset))
        {
            offset = 5;
        }

        var scheduledTime = DateTime.UtcNow.AddMinutes(offset);

        var newEvent = new ScheduledEvent
        {
            PetId = petId,
            Type = evtType,
            ScheduledTime = scheduledTime
        };
        _notificationService.AddScheduledEvent(newEvent);
        var pet = _petService.GetPetById(petId);

        Console.WriteLine($"Scheduled {evtType} event for Pet {pet!.Name} at {scheduledTime} (UTC).");
    }
    #endregion

    #region Inventory
    static void ManageInventory()
    {
        Console.WriteLine($"Current food stock: {_inventoryService.CurrentFoodQuantity} units");
        Console.WriteLine("1) Add new food stock");
        Console.WriteLine("0) Return to main menu");
        Console.Write("Choice: ");
        var c = Console.ReadLine();
        if (c == "1")
        {
            Console.Write("Enter amount of food to add: ");
            if (double.TryParse(Console.ReadLine(), out double addAmount))
            {
                _inventoryService.AddFoodStock(addAmount);
                Console.WriteLine($"Food stock updated. Current stock = {_inventoryService.CurrentFoodQuantity} units.");
            }
            else
            {
                Console.WriteLine("Invalid amount.");
            }
        }
    }
    #endregion

    #region Optional Seed Data
    static void SeedData()
    {
        // Add some pets
        var pet1 = _petService.AddPet("Max");
        var pet2 = _petService.AddPet("Bella");

        // Add some inventory
        _inventoryService.AddFoodStock(10.0); // 10 units of food

        // Add scheduled events
        _notificationService.AddScheduledEvent(new ScheduledEvent
        {
            PetId = pet1.Id,
            Type = EventType.Feed,
            ScheduledTime = DateTime.UtcNow.AddMinutes(2)
        });
        _notificationService.AddScheduledEvent(new ScheduledEvent
        {
            PetId = pet2.Id,
            Type = EventType.Walk,
            ScheduledTime = DateTime.UtcNow.AddMinutes(5)
        });
    }
    #endregion
}
