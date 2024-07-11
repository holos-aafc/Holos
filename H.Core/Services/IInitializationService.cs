using System.Collections.Generic;
using System.Windows.Controls;
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
        void InitializeManureMineralizationFractions(Farm farm);
        void InitializeManureMineralizationFractions(ManagementPeriod managementPeriod, FractionOfOrganicNitrogenMineralizedData  mineralizationFractions);
        void InitializeDefaultEmissionFactors(Farm farm, AnimalComponentBase animalComponent, ManagementPeriod managementPeriod);
        void ReinitializeBeddingMaterial(Farm farm);
        void InitializeBeddingMaterial(ManagementPeriod managementPeriod, Table_30_Default_Bedding_Material_Composition_Data data);
        void InitializeDefaultEmissionFactors(Farm farm);
        void InitializeMethaneProducingCapacity(Farm farm);
        void InitializeMethaneProducingCapacity(ManagementPeriod managementPeriod);
        void InitializeCattleFeedingActivity(Farm farm);
    }
}