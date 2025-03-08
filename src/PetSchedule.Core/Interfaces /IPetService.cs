using PetSchedule.Core.Entities;

namespace PetSchedule.Core.Interfaces;

public interface IPetService
{
    Pet AddPet(string name);
    IEnumerable<Pet> GetAllPets();
    Pet? GetPetById(int petId);

    // Recording feed
    FeedRecord AddFeedRecord(int petId, double amount);

    // Recording walk
    WalkRecord AddWalkRecord(int petId, DateTime start, DateTime end);
}
