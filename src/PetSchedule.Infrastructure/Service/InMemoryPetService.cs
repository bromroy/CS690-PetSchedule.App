using PetSchedule.Core.Entities;
using PetSchedule.Core.Interfaces;

namespace PetSchedule.Infrastructure.Service;

public class InMemoryPetService : IPetService
{
    private readonly List<Pet> _pets = new List<Pet>();
    private readonly List<FeedRecord> _feedRecords = new List<FeedRecord>();
    private readonly List<WalkRecord> _walkRecords = new List<WalkRecord>();

    private int _petIdCounter = 0;
    private int _feedIdCounter = 0;
    private int _walkIdCounter = 0;

    public Pet AddPet(string name)
    {
        var pet = new Pet
        {
            Id = ++_petIdCounter,
            Name = name
        };
        _pets.Add(pet);
        return pet;
    }

    public IEnumerable<Pet> GetAllPets()
    {
        return _pets;
    }

    public Pet? GetPetById(int petId)
    {
        return _pets.FirstOrDefault(p => p.Id == petId);
    }

    public FeedRecord AddFeedRecord(int petId, double amount)
    {
        var pet = GetPetById(petId);
        if (pet == null)
            throw new ArgumentException($"No pet found with ID {petId}");

        var record = new FeedRecord
        {
            Id = ++_feedIdCounter,
            PetId = petId,
            FeedTime = DateTime.UtcNow,
            Amount = amount
        };

        _feedRecords.Add(record);
        pet.FeedRecords.Add(record);

        return record;
    }

    public WalkRecord AddWalkRecord(int petId, DateTime start, DateTime end)
    {
        var pet = GetPetById(petId);
        if (pet == null)
            throw new ArgumentException($"No pet found with ID {petId}");

        var record = new WalkRecord
        {
            Id = ++_walkIdCounter,
            PetId = petId,
            WalkStart = start,
            WalkEnd = end
        };

        _walkRecords.Add(record);
        pet.WalkRecords.Add(record);

        return record;
    }
}