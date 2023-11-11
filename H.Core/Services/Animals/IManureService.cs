using System.Collections.Generic;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Animals;

namespace H.Core.Services.Animals
{
    public interface IManureService
    {
        void Initialize(Farm farm, List<AnimalComponentEmissionsResults> animalComponentEmissionsResults);
        ManureTank GetTank(AnimalType animalType, int year, Farm farm);
        List<AnimalType> GetValidManureTypes();
        List<AnimalType> GetManureTypesProducedOnFarm(Farm farm);
        double GetVolumeAvailableForExport(int year);
        double GetVolumeAvailableForExport(int year, Farm farm, AnimalType animalType);
        List<ManureApplicationTypes> GetValidManureApplicationTypes();
        List<ManureLocationSourceType> GetValidManureLocationSourceTypes();
        List<ManureStateType> GetValidManureStateTypes(Farm farm, ManureLocationSourceType locationSourceType, AnimalType animalType);
        void SetValidManureStateTypes(ManureItemBase manureItemBase, Farm farm);

        /// <summary>
        /// (kg)
        /// </summary>
        double GetTotalVolumeOfManureExported(int year, Farm farm);

        /// <summary>
        /// (kg)
        /// </summary>
        double GetTotalVolumeOfManureExported(int year, Farm farm, AnimalType animalType);
        int GetYearHighestVolumeRemaining(AnimalType animalType);
        DefaultManureCompositionData GetManureCompositionData(ManureItemBase manureItemBase, Farm farm);

        /// <summary>
        /// (kg)
        /// </summary>
        double GetTotalTANCreated(int year);

        /// <summary>
        /// (kg)
        /// </summary>
        double GetTotalTANCreated(int year, AnimalType animalType);

        /// <summary>
        /// (kg)
        /// </summary>
        double GetTotalNitrogenCreated(int year);

        /// <summary>
        /// (kg)
        /// </summary>
        double GetTotalNitrogenCreated(int year, AnimalType animalType);

        /// <summary>
        /// (kg)
        /// </summary>
        double GetTotalVolumeCreated(int year);

        /// <summary>
        /// (kg)
        /// </summary>
        double GetTotalVolumeCreated(int year, AnimalType animalType);

        /// <summary>
        /// Equation 4.6.1-8
        /// Equation 4.6.2-17
        /// Equation 4.6.2-18
        ///
        /// (kg N)
        /// </summary>
        double GetTotalNitrogenFromExportedManure(int year, Farm farm);

        double GetTotalNitrogenFromExportedManure(int year, Farm farm, AnimalType animalType);
        double GetTotalNitrogenFromManureImports(int year, Farm farm, AnimalType animalType);

        /// <summary>
        /// Equation 4.6.2-13
        /// 
        /// Nitrogen sum of all manure applications made to all fields from manure produced on the farm (does not include imported manure)
        ///
        /// (kg N)
        /// </summary>
        double GetTotalNitrogenAppliedToAllFields(int year);

        /// <summary>
        /// Equation 4.6.2-14
        /// 
        /// Stored nitrogen available for application to land minus manure applied to fields or exported
        ///
        /// (kg N)
        /// </summary>
        double GetTotalNitrogenRemaining(int year, Farm farm);

        List<AnimalType> GetManureTypesExported(Farm farm, int year);
        List<AnimalType> GetManureTypesImported(Farm farm, int year);
    }
}