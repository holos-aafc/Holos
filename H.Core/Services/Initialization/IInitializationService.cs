using System.Collections.Generic;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Animals;
using H.Core.Providers.Carbon;
using H.Core.Services.Initialization.Crops;

namespace H.Core.Services.Initialization
{
    public interface IInitializationService : ICropInitializationService
    {
        void CheckInitialization(Farm farm);
        void ReInitializeFarms(IEnumerable<Farm> farms);

        void InitializeManureCompositionData(Farm farm);
        void InitializeManureCompositionData(ManagementPeriod managementPeriod, DefaultManureCompositionData manureCompositionData);
        void InitializeMilkProduction(Farm farm);
        void InitializeManureMineralizationFractions(Farm farm);
        void InitializeManureMineralizationFractions(ManagementPeriod managementPeriod, FractionOfOrganicNitrogenMineralizedData  mineralizationFractions);
        void InitializeDefaultEmissionFactors(Farm farm, ManagementPeriod managementPeriod);
        void ReinitializeBeddingMaterial(Farm farm);
        void InitializeBeddingMaterial(ManagementPeriod managementPeriod, Table_30_Default_Bedding_Material_Composition_Data data);
        void InitializeDefaultEmissionFactors(Farm farm);
        void InitializeMethaneProducingCapacity(Farm farm);
        void InitializeMethaneProducingCapacity(ManagementPeriod managementPeriod);
        void InitializeCattleFeedingActivity(Farm farm);
        void InitializeParameterAdjustmenstForManure(Farm farm);
        void InitializeManureExcretionRate(ManagementPeriod managementPeriod);
        void InitializeManureExcretionRate(Farm farm);
        void InitializeAnnualEntericMethaneRate(Farm farm);
        void InitializeAnnualEntericMethaneRate(ManagementPeriod managementPeriod);
        void InitializeLivestockCoefficientSheep(Farm farm);
        void InitializeVolatileSolidsExcretion(Farm farm);
        void InitializeVolatileSolidsExcretion(ManagementPeriod managementPeriod, Province province);
        void InitializeBarnTemperature(Farm farm);
        void InitializeOtherLivestockDefaultCH4EmissionFactor(Farm farm);
    }
}
    
