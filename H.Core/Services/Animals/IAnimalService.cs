using System.Collections.Generic;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;

namespace H.Core.Services.Animals
{
    public interface IAnimalService
    {
        List<AnimalComponentEmissionsResults> GetAnimalResults(Farm farm);
        List<AnimalComponentEmissionsResults> GetAnimalResults(AnimalType animalType, Farm farm);
        AnimalGroupEmissionResults GetResultsForGroup(AnimalGroup animalGroup, Farm farm, AnimalComponentBase animalComponent);
        AnimalGroupEmissionResults GetResultsForManagementPeriod(AnimalGroup animalGroup, Farm farm, AnimalComponentBase animalComponent, ManagementPeriod managementPeriod);
    }
}