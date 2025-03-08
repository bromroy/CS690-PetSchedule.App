using PetSchedule.Core.Interfaces;
using PetSchedule.Infrastructure.Service;

namespace PetSchedule.Tests;


public class InventoryServiceTests
{
    private IInventoryService _inventory;

    public InventoryServiceTests()
    {
        _inventory = new InMemoryInventoryService();
    }

    [Fact]
    public void Should_Initialize_With_Zero_Food()
    {
        Assert.Equal(0.0, _inventory.CurrentFoodQuantity);
    }

    [Fact]
    public void Should_Add_Food_Stock()
    {
        // Arrange
        double amountToAdd = 5.0;

        // Act
        _inventory.AddFoodStock(amountToAdd);

        // Assert
        Assert.Equal(5.0, _inventory.CurrentFoodQuantity);
    }

    [Fact]
    public void Should_Throw_When_Adding_Negative_Food()
    {
        // Arrange
        double invalidAmount = -2.0;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _inventory.AddFoodStock(invalidAmount));
    }

    [Fact]
    public void Should_Use_Food_Stock_When_Available()
    {
        // Arrange
        _inventory.AddFoodStock(10.0);

        // Act
        bool success = _inventory.UseFoodStock(3.0);

        // Assert
        Assert.True(success);
        Assert.Equal(7.0, _inventory.CurrentFoodQuantity);
    }

    [Fact]
    public void Should_Use_Food_Stock_Exactly_Equal_To_Current()
    {
        // Arrange
        _inventory.AddFoodStock(5.0);

        // Act
        bool success = _inventory.UseFoodStock(5.0);

        // Assert
        Assert.True(success);
        Assert.Equal(0.0, _inventory.CurrentFoodQuantity);
    }

    [Fact]
    public void Should_Fail_Use_When_Not_Enough_Stock()
    {
        // Arrange
        _inventory.AddFoodStock(2.0);

        // Act
        bool success = _inventory.UseFoodStock(5.0);

        // Assert
        Assert.False(success);
        Assert.Equal(2.0, _inventory.CurrentFoodQuantity);
    }

    [Fact]
    public void Should_Throw_When_Using_Negative_Food()
    {
        // Arrange
        _inventory.AddFoodStock(5.0);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _inventory.UseFoodStock(-1.0));
    }
}
