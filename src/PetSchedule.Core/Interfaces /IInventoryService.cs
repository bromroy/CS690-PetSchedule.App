namespace PetSchedule.Core.Interfaces; 

public interface IInventoryService
{
    double CurrentFoodQuantity { get; }
    void AddFoodStock(double amount); // e.g., buy a new bag
    bool UseFoodStock(double amount); // e.g., feed the pet => reduce stock
}