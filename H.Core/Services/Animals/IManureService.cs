using System.Collections.Generic;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Animals;

namespace H.Core.Services.Animals
{
    public interface IManureService
    {
        void CalculateResults(Farm farm);
        ManureTank GetTank(AnimalType animalType, int year, Farm farm);
        List<AnimalType> GetValidManureTypes();
        List<AnimalType> GetManureTypesProducedOnFarm(Farm farm);
        double GetAmountAvailableForExport(int year, Farm farm);
        double GetAmountAvailableForExport(int year, Farm farm, AnimalType animalType);
        double GetTotalAmountOfManureExported(int year, Farm farm, AnimalType animalType);
        List<ManureApplicationTypes> GetValidManureApplicationTypes();
        List<ManureLocationSourceType> GetValidManureLocationSourceTypes();
        List<ManureStateType> GetValidManureStateTypes(Farm farm, ManureLocationSourceType locationSourceType, AnimalType animalType);
        void SetValidManureStateTypes(ManureItemBase manureItemBase, Farm farm);
        double GetTotalAmountOfManureExported(int year, Farm farm);
        int GetYearHighestVolumeRemaining();
        DefaultManureCompositionData GetManureCompositionData(ManureItemBase manureItemBase, Farm farm);
    }
}