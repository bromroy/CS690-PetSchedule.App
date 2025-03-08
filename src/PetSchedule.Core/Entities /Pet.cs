namespace PetSchedule.Core.Entities;

public class Pet
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // For a real-world domain, we often have a collection of feed/walk records:
    public ICollection<FeedRecord> FeedRecords { get; set; } = new List<FeedRecord>();
    public ICollection<WalkRecord> WalkRecords { get; set; } = new List<WalkRecord>();
}
