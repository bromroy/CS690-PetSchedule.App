namespace PetSchedule.Core.Entities;

public class WalkRecord
{
    public int Id { get; set; }
    public DateTime WalkStart { get; set; }
    public DateTime WalkEnd { get; set; }
    public int PetId { get; set; }
}
