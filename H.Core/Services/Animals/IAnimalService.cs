using System.Collections.Generic;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;

namespace H.Core.Services.Animals
{
    public interface IAnimalService
    {
        List<AnimalComponentEmissionsResults> GetAnimalResults(Farm farm);
        List<AnimalComponentEmissionsResults> GetAnimalResults(AnimalType animalType, Farm farm);
    }
}