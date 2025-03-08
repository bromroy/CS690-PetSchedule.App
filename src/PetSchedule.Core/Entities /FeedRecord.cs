namespace PetSchedule.Core.Entities;

public class FeedRecord
{
    public int Id { get; set; }
    public DateTime FeedTime { get; set; }
    public double Amount { get; set; } // e.g., cups of food
    public int PetId { get; set; }
}
