using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Providers.Animals;
using H.Core.Providers.Climate;
using H.Core.Providers.Soil;

namespace H.Core.Services.Initialization.Animals
{
    public interface IAnimalInitializationService
    {
        /// <summary>
        /// Reinitialize the DailyMethaneEmissionRate for each ManagementPeriod of each farm
        /// </summary>
        /// <param name="farm"> Contains the ManagementPeriod.ManureDetails.DailyMAnureMethaneEmissionRate that needs to be reinitialized to default</param>
        void InitializeOtherLivestockDefaultCH4EmissionFactor(Farm farm);

        /// <summary>
        /// Reinitialize each <see cref="ManagementPeriod"/>'s volatile solid excretion manure detail within a <see cref="Farm"/>
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing <see cref="ManagementPeriod"/>'s to be reinitialized</param>
        void InitializeVolatileSolidsExcretion(Farm farm);

        /// <summary>
        /// Reinitialize the <see cref="ManagementPeriod"/> volatile solid excretion manure detail
        /// </summary>
        /// <param name="managementPeriod">The <see cref="ManagementPeriod"/> to be reinitialized</param>
        /// <param name="province">The <see cref="Province"/> used to get correct values from lookup table</param>
        void InitializeVolatileSolidsExcretion(ManagementPeriod managementPeriod, Province province);

        /// <summary>
        /// Initialize the default annual enteric methane rate for all <see cref="ManagementPeriod"/>s associated with this <see cref="Farm"/>.
        /// </summary>Whi
        /// <param name="farm">The <see cref="Farm"/> containing the <see cref="ManagementPeriod"/>s to initialize</param>
        void InitializeAnnualEntericMethaneRate(Farm farm);

        /// <summary>
        /// Initialize the default annual enteric methane rate for the <see cref="ManagementPeriod"/>.
        /// </summary>
        /// <param name="managementPeriod">The <see cref="ManagementPeriod"/> to initialize with a default <see cref="ManureDetails.YearlyEntericMethaneRate"/></param>
        void InitializeAnnualEntericMethaneRate(ManagementPeriod managementPeriod);

        /// <summary>
        /// Initialize the default <see cref="ManureDetails.MethaneProducingCapacityOfManure"/> for all <see cref="ManagementPeriod"/>s associated with this <see cref="Farm"/>.
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the <see cref="ManagementPeriod"/>s to initialize</param>
        void InitializeMethaneProducingCapacity(Farm farm);

        /// <summary>
        /// Initialize the default <see cref="ManureDetails.MethaneProducingCapacityOfManure"/> for the <see cref="ManagementPeriod"/>.
        /// </summary>
        /// <param name="managementPeriod">The <see cref="ManagementPeriod"/> to initialize with a default <see cref="ManureDetails.MethaneProducingCapacityOfManure"/></param>
        void InitializeMethaneProducingCapacity(ManagementPeriod managementPeriod);

        /// <summary>
        /// Reinitialize the <see cref="Table_30_Default_Bedding_Material_Composition_Data"/> for all <see cref="ManagementPeriod"/>s for the <see cref="Farm"/>.
        /// </summary>
        /// <param name="farm">The farm containing the <see cref="ManagementPeriod"/>s to initialize with new defaults</param>
        void ReinitializeBeddingMaterial(Farm farm);

        /// <summary>
        /// Reinitialize the <see cref="Table_30_Default_Bedding_Material_Composition_Data"/> for the <see cref="ManagementPeriod"/>.
        /// </summary>
        /// <param name="managementPeriod">The <see cref="ManagementPeriod"/> that will be reinitialized to new default values</param>
        /// <param name="data">The defaults to use for the initialization</param>
        void InitializeBeddingMaterial(ManagementPeriod managementPeriod, Table_30_Default_Bedding_Material_Composition_Data data);

        /// <summary>
        /// Initialize the default manure mineralization fractions for all <see cref="ManagementPeriod"/>s associated with this <see cref="Farm"/>.
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the <see cref="ManagementPeriod"/>s to initialize</param>
        void InitializeManureMineralizationFractions(Farm farm);

        /// <summary>
        /// Initialize the manure fractions for the selected <see cref="ManagementPeriod"/>.
        /// </summary>
        /// <param name="managementPeriod">The <see cref="ManagementPeriod"/> that will have it's values reset to system defaults</param>
        void InitializeManureMineralizationFractions(ManagementPeriod managementPeriod);

        /// <summary>
        /// Initialize the default manure excretion rate for all <see cref="ManagementPeriod"/>s associated with this <see cref="Farm"/>.
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the <see cref="ManagementPeriod"/>s to initialize</param>
        void InitializeManureExcretionRate(Farm farm);

        /// <summary>
        /// Initialize the default <see cref="ManureDetails.ManureExcretionRate"/> for the <see cref="ManagementPeriod"/>.
        /// </summary>
        /// <param name="managementPeriod">The <see cref="ManagementPeriod"/> to initialize with a default <see cref="ManureDetails.ManureExcretionRate"/></param>
        void InitializeManureExcretionRate(ManagementPeriod managementPeriod);

        /// <summary>
        /// Initialize all <see cref="ClimateData.BarnTemperatureData"/> for all <see cref="ManagementPeriod"/>s in the <see cref="Farm"/>.
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> to initialize with new defaults</param>
        void InitializeBarnTemperature(Farm farm);

        /// <summary>
        /// Reinitialize the MilkProduction value for each ManagementPeriod for each animalGroup in the DairyComponent of a <see cref="Farm"/> with new default values from table 21.
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> that will be reinitialized to new default value for the MilkProduction</param>
        void InitializeMilkProduction(Farm farm);

        /// <summary>
        /// Reinitialize the default <see cref="DefaultManureCompositionData"/> for all <see cref="ManagementPeriod"/>s associated with this <see cref="Farm"/>.
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the <see cref="ManagementPeriod"/>s to initialize</param>
        void InitializeManureCompositionData(Farm farm);

        /// <summary>
        /// Initialize the manure <see cref="DefaultManureCompositionData"/> for the selected <see cref="ManagementPeriod"/>.
        /// </summary>
        /// <param name="managementPeriod">The <see cref="ManagementPeriod"/> that will have it's values reset to system defaults</param>
        /// <param name="manureCompositionData">The <see cref="DefaultManureCompositionData"/> containing the new default values to use</param>
        void InitializeManureCompositionData(ManagementPeriod managementPeriod, DefaultManureCompositionData manureCompositionData);

        /// <summary>
        /// Reinitializes the values for Ewes, ram and weanedLamb from Table 22
        /// </summary>
        /// <param name="farm">Contains the coefficient(s), weight(s) and wool production details that need to be reinitialized</param>
        void InitializeLivestockCoefficientSheep(Farm farm);

        /// <summary>
        /// Initialize the default emission factors for all <see cref="ManagementPeriod"/>s associated with this <see cref="Farm"/>.
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> that will be reinitialized to new default values</param>
        void InitializeDefaultEmissionFactors(Farm farm);

        /// <summary>
        /// Initialize the default emission factors for the <see cref="ManagementPeriod"/>.
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the <see cref="ManagementPeriod"/></param>
        /// <param name="managementPeriod">The <see cref="ManagementPeriod"/> that will be reinitialized to new default values</param>
        void InitializeDefaultEmissionFactors(
            Farm farm,
            ManagementPeriod managementPeriod);

        /// <summary>
        /// Reinitialize the Beef_Dairy_Cattle_Feeding_Activity_Coefficient object
        /// </summary>
        /// <param name="farm"> Contains the ActivityCoefficientFeedingSituation of each HousingDetails of each ManagementPeriod of each <see cref="farm"/></param>
        void InitializeCattleFeedingActivity(Farm farm);

        void InitializeFeedingActivityCoefficient(ManagementPeriod managementPeriod);
        void InitializeMilkProduction(ManagementPeriod managementPeriod, SoilData soilData);
        void InitializeLeachingFraction(Farm farm, ManagementPeriod managementPeriod);
        void InitializeLeachingFraction(Farm farm);
    }
}