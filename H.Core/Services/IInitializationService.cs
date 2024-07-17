using System.Collections.Generic;
using System.Windows.Controls;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Animals;

namespace H.Core.Services
{
    public interface IInitializationService
    {
        void CheckInitialization(Farm farm);
        void ReInitializeFarms(IEnumerable<Farm> farms);

        void InitializeManureCompositionData(Farm farm);
        void InitializeManureCompositionData(ManagementPeriod managementPeriod, DefaultManureCompositionData manureCompositionData);
        void InitializeMilkProduction(Farm farm);
        void InitializeFuelEnergy(Farm farm);
        void InitializeFuelEnergy(Farm farm, CropViewItem viewItem);
        void InitializeHerbicideEnergy(Farm farm);
        void InitializeHerbicideEnergy(Farm farm, CropViewItem viewItem);
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
        void InitializePercentageReturns(Farm farm, CropViewItem viewItem);
    }
}
    
