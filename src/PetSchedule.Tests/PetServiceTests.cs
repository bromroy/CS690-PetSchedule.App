using PetSchedule.Core.Entities;
using PetSchedule.Core.Interfaces;
using PetSchedule.Infrastructure.Service;

namespace PetSchedule.Tests;

public class PetServiceTests
{
    private IPetService _petService;

    public PetServiceTests()
    {
        // Fresh instance for each test to avoid cross-test pollution
        _petService = new InMemoryPetService();
    }

    [Fact]
    public void Should_Add_New_Pet()
    {
        // Arrange
        string petName = "Max";

        // Act
        Pet newPet = _petService.AddPet(petName);

        // Assert
        Assert.NotNull(newPet);
        Assert.Equal(petName, newPet.Name);
        Assert.True(newPet.Id > 0);
    }

    [Fact]
    public void Should_Return_All_Pets()
    {
        // Arrange
        _petService.AddPet("Rocky");
        _petService.AddPet("Daisy");

        // Act
        var allPets = _petService.GetAllPets();

        // Assert
        Assert.Equal(2, allPets.Count());
    }

    [Fact]
    public void Should_Get_Pet_By_Id()
    {
        // Arrange
        var pet1 = _petService.AddPet("Bella");
        var pet2 = _petService.AddPet("Charlie");

        // Act
        var foundPet = _petService.GetPetById(pet2.Id);

        // Assert
        Assert.NotNull(foundPet);
        Assert.Equal("Charlie", foundPet?.Name);
    }

    [Fact]
    public void Should_Return_Null_When_Pet_Not_Found()
    {
        // Arrange
        // no pets added

        // Act
        var pet = _petService.GetPetById(999);

        // Assert
        Assert.Null(pet);
    }

    [Fact]
    public void Should_Add_FeedRecord()
    {
        // Arrange
        var pet = _petService.AddPet("Bella");

        // Act
        var feedRecord = _petService.AddFeedRecord(pet.Id, 2.5);

        // Assert
        Assert.NotNull(feedRecord);
        Assert.Equal(2.5, feedRecord.Amount);
        Assert.Equal(pet.Id, feedRecord.PetId);

        // Also ensure it's reflected in the pet's FeedRecords collection
        var updatedPet = _petService.GetPetById(pet.Id);
        Assert.Single(updatedPet!.FeedRecords);
    }

    [Fact]
    public void Should_Throw_When_Pet_Not_Found_For_Feed()
    {
        // Arrange
        int invalidPetId = 999;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _petService.AddFeedRecord(invalidPetId, 1.0));
    }

    [Fact]
    public void Should_Add_WalkRecord()
    {
        // Arrange
        var pet = _petService.AddPet("Luna");
        DateTime start = DateTime.UtcNow;
        DateTime end = start.AddMinutes(30);

        // Act
        var walkRecord = _petService.AddWalkRecord(pet.Id, start, end);

        // Assert
        Assert.NotNull(walkRecord);
        Assert.Equal(pet.Id, walkRecord.PetId);
        Assert.Equal(start, walkRecord.WalkStart);
        Assert.Equal(end, walkRecord.WalkEnd);

        // Also check the pet's WalkRecords
        var updatedPet = _petService.GetPetById(pet.Id);
        Assert.Single(updatedPet!.WalkRecords);
    }

    [Fact]
    public void Should_Throw_When_Pet_Not_Found_For_Walk()
    {
        // Arrange
        var start = DateTime.UtcNow;
        var end = start.AddMinutes(15);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _petService.AddWalkRecord(999, start, end));
    }
}
