using PetSchedule.Core.Interfaces;

namespace PetSchedule.Infrastructure.Service
{
    public class InMemoryInventoryService : IInventoryService
    {
        private double _currentFoodQuantity;

        public double CurrentFoodQuantity => _currentFoodQuantity;

        public void AddFoodStock(double amount)
        {
            if (amount < 0) throw new ArgumentException("Cannot add negative food quantity");
            _currentFoodQuantity += amount;
        }

        public bool UseFoodStock(double amount)
        {
            if (amount < 0) throw new ArgumentException("Cannot use negative food quantity");
            if (amount > _currentFoodQuantity)
            {
                return false; // not enough food
            }

            _currentFoodQuantity -= amount;
            return true;
        }

    }

}