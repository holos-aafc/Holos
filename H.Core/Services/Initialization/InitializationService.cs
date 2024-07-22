using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using H.Core.Calculators.Carbon;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.Animals.Dairy;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Animals;
using H.Core.Providers.Carbon;
using H.Core.Providers.Climate;
using H.Core.Providers.Energy;
using H.Core.Providers.Irrigation;
using H.Core.Providers.Nitrogen;
using H.Core.Providers.Plants;
using H.Core.Providers.Soil;
using H.Core.Services.LandManagement;
using H.Infrastructure;

namespace H.Core.Services.Initialization
{
    public partial class InitializationService : IInitializationService
    {
        #region Fields

        private readonly IIndoorTemperatureProvider _indoorTemperatureProvider;
        private readonly Table_21_Average_Milk_Production_Dairy_Cows_Provider _averageMilkProductionDairyCowsProvider;
        private readonly Table_6_Manure_Types_Default_Composition_Provider _defaultManureCompositionProvider;
        private readonly Table_44_Fraction_OrganicN_Mineralized_As_Tan_Provider _fractionOrganicNMineralizedAsTanProvider;
        private readonly Table_36_Livestock_Emission_Conversion_Factors_Provider _livestockEmissionConversionFactorsProvider;
        private readonly Table_50_Fuel_Energy_Estimates_Provider _fuelEnergyEstimatesProvider;
        private readonly Table_16_Livestock_Coefficients_BeefAndDairy_Cattle_Provider _beefAndDairyCattleProvider;
        private readonly Table_22_Livestock_Coefficients_Sheep_Provider _sheepProvider;
        private readonly Table_29_Default_Manure_Excreted_Provider _defaultManureExcretionRateProvider;
        private readonly Table_30_Default_Bedding_Material_Composition_Provider _beddingMaterialCompositionProvider;
        private readonly List<Table_21_Average_Milk_Production_Dairy_Cows_Data> _milkProductionDataList;
        private readonly Table_35_Methane_Producing_Capacity_Default_Values_Provider _defaultMethaneProducingCapacityProvider;
        private readonly Table_17_Beef_Dairy_Cattle_Feeding_Activity_Coefficient_Provider _beefDairyCattleFeedingActivityCoefficientProvider;
        private readonly Table_51_Herbicide_Energy_Estimates_Provider _herbicideEnergyEstimatesProvider;
        private readonly Table_27_Enteric_CH4_Swine_Poultry_OtherLivestock_Provider _entericMethaneProvider;
        private readonly Table_31_Swine_VS_Excretion_For_Diets_Provider _volatileExcretionForDietsProvider;
        private readonly Table_4_Monthly_Irrigation_Water_Application_Provider _irrigationWaterApplicationProvider;
        private readonly Table_38_OtherLivestock_Default_CH4_Emission_Factors_Provider _otherLivestockDefaultCH4EmissionFactorsProvider;
        private readonly Table_38_OtherLivestock_Default_CH4_Emission_Factors_Provider _otherLivestockDefaultCh4EmissionFactorsProvider;
        private readonly Table_7_Relative_Biomass_Information_Provider _relativeBiomassInformationProvider;
        private readonly SmallAreaYieldProvider _smallAreaYieldProvider;
        private readonly ICBMCarbonInputCalculator _icbmCarbonInputCalculator;
        private readonly NitogenFixationProvider _nitrogenFixationProvider;
        private readonly IrrigationService _irrigationService;
        private readonly Table_9_Nitrogen_Lignin_Content_In_Crops_Provider _slopeProviderTable;
        private readonly Table_60_Utilization_Rates_For_Livestock_Grazing_Provider _utilizationRatesForLivestockGrazingProvider;

        #endregion

        #region Constructors

        public InitializationService()
        {
            _indoorTemperatureProvider = new Table_63_Indoor_Temperature_Provider();
            _averageMilkProductionDairyCowsProvider = new Table_21_Average_Milk_Production_Dairy_Cows_Provider();
            _defaultManureCompositionProvider = new Table_6_Manure_Types_Default_Composition_Provider();
            _fractionOrganicNMineralizedAsTanProvider = new Table_44_Fraction_OrganicN_Mineralized_As_Tan_Provider();
            _livestockEmissionConversionFactorsProvider = new Table_36_Livestock_Emission_Conversion_Factors_Provider(); ;
            _fuelEnergyEstimatesProvider = new Table_50_Fuel_Energy_Estimates_Provider();
            _beefAndDairyCattleProvider = new Table_16_Livestock_Coefficients_BeefAndDairy_Cattle_Provider();
            _sheepProvider = new Table_22_Livestock_Coefficients_Sheep_Provider();
            _beddingMaterialCompositionProvider = new Table_30_Default_Bedding_Material_Composition_Provider();
            _milkProductionDataList = _averageMilkProductionDairyCowsProvider.ReadFile();
            _defaultMethaneProducingCapacityProvider = new Table_35_Methane_Producing_Capacity_Default_Values_Provider();
            _beefDairyCattleFeedingActivityCoefficientProvider = new Table_17_Beef_Dairy_Cattle_Feeding_Activity_Coefficient_Provider();
            _herbicideEnergyEstimatesProvider = new Table_51_Herbicide_Energy_Estimates_Provider();
            _defaultManureExcretionRateProvider = new Table_29_Default_Manure_Excreted_Provider();
            _entericMethaneProvider = new Table_27_Enteric_CH4_Swine_Poultry_OtherLivestock_Provider();
            _volatileExcretionForDietsProvider = new Table_31_Swine_VS_Excretion_For_Diets_Provider();
            _otherLivestockDefaultCH4EmissionFactorsProvider = new Table_38_OtherLivestock_Default_CH4_Emission_Factors_Provider();
            _icbmCarbonInputCalculator = new ICBMCarbonInputCalculator();
            _nitrogenFixationProvider = new NitogenFixationProvider();
            _irrigationWaterApplicationProvider = new Table_4_Monthly_Irrigation_Water_Application_Provider();
            _otherLivestockDefaultCh4EmissionFactorsProvider = new Table_38_OtherLivestock_Default_CH4_Emission_Factors_Provider();
            _relativeBiomassInformationProvider = new Table_7_Relative_Biomass_Information_Provider();
            _irrigationService = new IrrigationService();
            _smallAreaYieldProvider = new SmallAreaYieldProvider();
            _slopeProviderTable = new Table_9_Nitrogen_Lignin_Content_In_Crops_Provider(); 
            _utilizationRatesForLivestockGrazingProvider = new Table_60_Utilization_Rates_For_Livestock_Grazing_Provider();
        }
        
        #endregion

        #region Public Methods

        public void CheckInitialization(Farm farm)
        {
            if (farm == null)
            {
                return;
            }

            if (farm.DefaultSoilData == null)
            {
                return;
            }

            if (farm.ClimateData == null)
            {
                return;
            }

            var climateData = farm.ClimateData;

            var barnTemperature = climateData.BarnTemperatureData;
            if (barnTemperature == null || barnTemperature.IsInitialized == false)
            {
                this.InitializeBarnTemperature(farm);
            }
        }

        public void ReInitializeFarms(IEnumerable<Farm> farms)
        {
            foreach (var farm in farms)
            {
                // Defaults (carbon concentration)
                this.InitializeCarbonConcentration(farm);

                // Nitrogen Fixation
                this.InitializeNitrogenFixation(farm);

                // Table 4
                this.InitializeIrrigationWaterApplication(farm);

                // Table 6
                this.InitializeManureCompositionData(farm);

                // Table 17
                this.InitializeCattleFeedingActivity(farm);

                // Table 21
                this.InitializeMilkProduction(farm);

                // Table 22
                this.InitializeLivestockCoefficientSheep(farm);

                // Table 27
                this.InitializeAnnualEntericMethaneRate(farm);

                // Table 29
                this.InitializeManureExcretionRate(farm);

                // Table 31
                this.InitializeVolatileSolidsExcretion(farm);

                // Table 35
                this.InitializeMethaneProducingCapacity(farm);

                // Table 36
                this.InitializeDefaultEmissionFactors(farm);

                //Table 38
                this.InitializeOtherLivestockDefaultCH4EmissionFactor(farm);

                // Table 44
                this.InitializeManureMineralizationFractions(farm);

                // Table 50
                this.InitializeFuelEnergy(farm);

                // Table 51
                this.InitializeHerbicideEnergy(farm);

                // Table 63
                this.InitializeBarnTemperature(farm);
            }
        }

        /// <summary>
        /// Reinitialize each <see cref="CropViewItem"/>'s irrigation properties with a <see cref="Farm"/>
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing <see cref="CropViewItem"/>'s to be reinitialized</param>
        public void InitializeIrrigationWaterApplication(Farm farm)
        {
            var viewItems = farm.GetCropDetailViewItems();
            foreach (var viewItem in viewItems)
            {
                InitializeIrrigationWaterApplication(farm, viewItem);
            }
        }

        /// <summary>
        /// Reinitialize the <see cref="CropViewItem"/> irrigation properties
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> that contains the climate data and province data required for the lookup table</param>
        /// <param name="viewItem">The <see cref="CropViewItem"/> to have its irrigation properties reinitialized</param>
        public void InitializeIrrigationWaterApplication(Farm farm, CropViewItem viewItem)
        {
            viewItem.GrowingSeasonIrrigation = _irrigationService.GetGrowingSeasonIrrigation(farm, viewItem);
            viewItem.AmountOfIrrigation = _irrigationService.GetDefaultIrrigationForYear(farm, viewItem.Year);
        }

        /// <summary>
        /// Reinitialize the DailyMethaneEmissionRate for each ManagementPeriod of each farm
        /// </summary>
        /// <param name="farm"> Contains the ManagementPeriod.ManureDetails.DailyMAnureMethaneEmissionRate that needs to be reinitialized to default</param>
        public void InitializeOtherLivestockDefaultCH4EmissionFactor(Farm farm)
        {
            if (farm != null)
            {
                foreach (var component in farm.AnimalComponents)
                {
                    if (component.Groups != null)
                    {
                        foreach (var animalGroup in component.Groups)
                        {
                            foreach (var managementPeriod in animalGroup.ManagementPeriods)
                            {
                                managementPeriod.ManureDetails.DailyManureMethaneEmissionRate = _otherLivestockDefaultCh4EmissionFactorsProvider.GetDailyManureMethaneEmissionRate(animalGroup.GroupType);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Reinitialize each <see cref="ManagementPeriod"/>'s volatile solid excretion manure detail within a <see cref="Farm"/>
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing <see cref="ManagementPeriod"/>'s to be reinitialized</param>
        public void InitializeVolatileSolidsExcretion(Farm farm)
        {
            if (farm != null && farm.DefaultSoilData != null)
            {
                var province = farm.DefaultSoilData.Province;

                foreach (var managementPeriod in farm.GetAllManagementPeriods().Where(x => AnimalTypeExtensions.IsSwineType(x.AnimalType)))
                {
                    this.InitializeVolatileSolidsExcretion(managementPeriod, province);
                }
            }
        }

        /// <summary>
        /// Reinitialize the <see cref="ManagementPeriod"/> volatile solid excretion manure detail
        /// </summary>
        /// <param name="managementPeriod">The <see cref="ManagementPeriod"/> to be reinitialized</param>
        /// <param name="province">The <see cref="Province"/> used to get correct values from lookup table</param>
        public void InitializeVolatileSolidsExcretion(ManagementPeriod managementPeriod, Province province)
        {
            if (managementPeriod != null && managementPeriod.ManureDetails != null && managementPeriod.AnimalType.IsSwineType())
            {
                managementPeriod.ManureDetails.VolatileSolidExcretion = _volatileExcretionForDietsProvider.Get(province, managementPeriod.AnimalType);
            }
        }

        /// <summary>
        /// Initialize the default annual enteric methane rate for all <see cref="ManagementPeriod"/>s associated with this <see cref="Farm"/>.
        /// </summary>Whi
        /// <param name="farm">The <see cref="Farm"/> containing the <see cref="ManagementPeriod"/>s to initialize</param>
        public void InitializeAnnualEntericMethaneRate(Farm farm)
        {
            if (farm != null)
            {
                foreach (var managementPeriod in farm.GetAllManagementPeriods())
                {
                    this.InitializeAnnualEntericMethaneRate(managementPeriod);
                }
            }
        }

        /// <summary>
        /// Initialize the default annual enteric methane rate for the <see cref="ManagementPeriod"/>.
        /// </summary>
        /// <param name="managementPeriod">The <see cref="ManagementPeriod"/> to initialize with a default <see cref="ManureDetails.YearlyEntericMethaneRate"/></param>
        public void InitializeAnnualEntericMethaneRate(ManagementPeriod managementPeriod)
        {
            if (managementPeriod != null && managementPeriod.ManureDetails != null)
            {
                managementPeriod.ManureDetails.YearlyEntericMethaneRate = _entericMethaneProvider.GetAnnualEntericMethaneEmissionRate(managementPeriod);
            }
        }

        /// <summary>
        /// Initialize the default manure excretion rate for all <see cref="ManagementPeriod"/>s associated with this <see cref="Farm"/>.
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the <see cref="ManagementPeriod"/>s to initialize</param>
        public void InitializeManureExcretionRate(Farm farm)
        {

            if (farm != null)
            {
                foreach (var managementPeriod in farm.GetAllManagementPeriods())
                {
                    this.InitializeManureExcretionRate(managementPeriod);
                }
            }
        }

        /// <summary>
        /// Initialize the default <see cref="ManureDetails.ManureExcretionRate"/> for the <see cref="ManagementPeriod"/>.
        /// </summary>
        /// <param name="managementPeriod">The <see cref="ManagementPeriod"/> to initialize with a default <see cref="ManureDetails.ManureExcretionRate"/></param>
        public void InitializeManureExcretionRate(ManagementPeriod managementPeriod)
        {
            if (managementPeriod != null && managementPeriod.HousingDetails != null)
            {
                managementPeriod.ManureDetails.ManureExcretionRate = _defaultManureExcretionRateProvider.GetManureExcretionRate(managementPeriod.AnimalType);
            }
        }

        /// <summary>
        /// Initialize the default <see cref="ManureDetails.MethaneProducingCapacityOfManure"/> for all <see cref="ManagementPeriod"/>s associated with this <see cref="Farm"/>.
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the <see cref="ManagementPeriod"/>s to initialize</param>
        public void InitializeMethaneProducingCapacity(Farm farm)
        {
            if (farm != null)
            {
                foreach (var managementPeriod in farm.GetAllManagementPeriods())
                {
                    this.InitializeMethaneProducingCapacity(managementPeriod);
                }
            }
        }

        /// <summary>
        /// Initialize the default <see cref="ManureDetails.MethaneProducingCapacityOfManure"/> for the <see cref="ManagementPeriod"/>.
        /// </summary>
        /// <param name="managementPeriod">The <see cref="ManagementPeriod"/> to initialize with a default <see cref="ManureDetails.MethaneProducingCapacityOfManure"/></param>
        public void InitializeMethaneProducingCapacity(ManagementPeriod managementPeriod)
        {
            if (managementPeriod != null && managementPeriod.ManureDetails != null)
            {
                var capacity = _defaultMethaneProducingCapacityProvider.GetMethaneProducingCapacityOfManure(managementPeriod.AnimalType);

                managementPeriod.ManureDetails.MethaneProducingCapacityOfManure = capacity;
            }
        }

        /// <summary>
        /// Reinitialize the default <see cref="DefaultManureCompositionData"/> for all <see cref="ManagementPeriod"/>s associated with this <see cref="Farm"/>.
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the <see cref="ManagementPeriod"/>s to initialize</param>
        public void InitializeManureCompositionData(Farm farm)
        {
            if (farm != null)
            {
                var manureCompositionData = _defaultManureCompositionProvider.ManureCompositionData;

                farm.DefaultManureCompositionData.Clear();
                farm.DefaultManureCompositionData.AddRange(manureCompositionData);

                foreach (var managementPeriod in farm.GetAllManagementPeriods())
                {
                    var defaults = _defaultManureCompositionProvider.GetManureCompositionDataByType(managementPeriod.AnimalType, managementPeriod.ManureDetails.StateType);

                    this.InitializeManureCompositionData(managementPeriod, defaults);
                }
            }
        }

        /// <summary>
        /// Initialize the manure <see cref="DefaultManureCompositionData"/> for the selected <see cref="ManagementPeriod"/>.
        /// </summary>
        /// <param name="managementPeriod">The <see cref="ManagementPeriod"/> that will have it's values reset to system defaults</param>
        /// <param name="manureCompositionData">The <see cref="DefaultManureCompositionData"/> containing the new default values to use</param>
        public void InitializeManureCompositionData(ManagementPeriod managementPeriod, DefaultManureCompositionData manureCompositionData)
        {
            if (managementPeriod != null &&
                managementPeriod.ManureDetails != null &&
                manureCompositionData != null)
            {
                managementPeriod.ManureDetails.FractionOfPhosphorusInManure = manureCompositionData.PhosphorusFraction;
                managementPeriod.ManureDetails.FractionOfCarbonInManure = manureCompositionData.CarbonFraction;
                managementPeriod.ManureDetails.FractionOfNitrogenInManure = manureCompositionData.NitrogenFraction;
            }
        }

        /// <summary>
        /// Initialize the default manure mineralization fractions for all <see cref="ManagementPeriod"/>s associated with this <see cref="Farm"/>.
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the <see cref="ManagementPeriod"/>s to initialize</param>
        public void InitializeManureMineralizationFractions(Farm farm)
        {
            if (farm != null)
            {
                foreach (var managementPeriod in farm.GetAllManagementPeriods())
                {
                    var fractions = _fractionOrganicNMineralizedAsTanProvider.GetByStorageType(managementPeriod.ManureDetails.StateType, managementPeriod.AnimalType);
                    this.InitializeManureMineralizationFractions(managementPeriod, fractions);
                }
            }
        }

        /// <summary>
        /// Initialize the manure fractions for the selected <see cref="ManagementPeriod"/>.
        /// </summary>
        /// <param name="managementPeriod">The <see cref="ManagementPeriod"/> that will have it's values reset to system defaults</param>
        /// <param name="mineralizationFractions">The <see cref="FractionOfOrganicNitrogenMineralizedData"/> containing the new default values to use</param>
        public void InitializeManureMineralizationFractions(ManagementPeriod managementPeriod, FractionOfOrganicNitrogenMineralizedData mineralizationFractions)
        {
            if (managementPeriod != null &&
                managementPeriod.ManureDetails != null &&
                mineralizationFractions != null)
            {
                managementPeriod.ManureDetails.FractionOfOrganicNitrogenImmobilized = mineralizationFractions.FractionImmobilized;
                managementPeriod.ManureDetails.FractionOfOrganicNitrogenNitrified = mineralizationFractions.FractionNitrified;
                managementPeriod.ManureDetails.FractionOfOrganicNitrogenMineralized = mineralizationFractions.FractionMineralized;
            }
        }

        /// <summary>
        /// Reinitialize each <see cref="CropViewItem"/> within <see cref="Farm"/> with new default values
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> that will be reinitialized to new default values</param>
        public void InitializeFuelEnergy(Farm farm)
        {
            var viewItems = farm.GetCropDetailViewItems();
            foreach (var viewItem in viewItems)
            {
                InitializeFuelEnergy(farm, viewItem);
            }
        }

        /// <summary>
        /// Reinitialize the <see cref="CropViewItem"/> from the selected <see cref="Farm"/> with new default values
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant data to pass into <see cref="Table_50_Fuel_Energy_Estimates_Provider"/></param>
        /// <param name="viewItem">The <see cref="CropViewItem"/> that will have its values reset with new default values</param>
        public void InitializeFuelEnergy(Farm farm, CropViewItem viewItem)
        {
            var soilData = farm.GetPreferredSoilData(viewItem);
            var fuelEnergyEstimates = _fuelEnergyEstimatesProvider.GetFuelEnergyEstimatesDataInstance(
                province: soilData.Province,
                soilCategory: soilData.SoilFunctionalCategory,
                tillageType: viewItem.TillageType,
                cropType: viewItem.CropType);

            if (fuelEnergyEstimates != null)
            {
                viewItem.FuelEnergy = fuelEnergyEstimates.FuelEstimate;
            }
        }

        /// <summary>
        /// Reinitialize each <see cref="CropViewItem"/> within <see cref="Farm"/> with new default values
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> that will be reinitialized to new default values</param>
        public void InitializeHerbicideEnergy(Farm farm)
        {
            var viewItems = farm.GetCropDetailViewItems();
            foreach (var viewItem in viewItems)
            {
                InitializeHerbicideEnergy(farm, viewItem);
            }
        }

        /// <summary>
        /// Reinitialize the <see cref="CropViewItem"/> from the selected <see cref="Farm"/> with new default values
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant data to pass into <see cref="Table_51_Herbicide_Energy_Estimates_Provider"/></param>
        /// <param name="viewItem">The <see cref="CropViewItem"/> that will have its values reset with new default values</param>
        public void InitializeHerbicideEnergy(Farm farm, CropViewItem viewItem)
        {
            var soilData = farm.GetPreferredSoilData(viewItem);
            var herbicideEnergyEstimates = _herbicideEnergyEstimatesProvider.GetHerbicideEnergyDataInstance(
                provinceName: soilData.Province,
                soilCategory: soilData.SoilFunctionalCategory,
                tillageType: viewItem.TillageType,
                cropType: viewItem.CropType);

            if (herbicideEnergyEstimates != null)
            {
                viewItem.HerbicideEnergy = herbicideEnergyEstimates.HerbicideEstimate;
            }
        }

        /// <summary>
        /// Initialize the default emission factors for all <see cref="ManagementPeriod"/>s associated with this <see cref="Farm"/>.
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> that will be reinitialized to new default values</param>
        public void InitializeDefaultEmissionFactors(Farm farm)
        {
            if (farm != null)
            {
                foreach (var managementPeriod in farm.GetAllManagementPeriods())
                {
                    this.InitializeDefaultEmissionFactors(farm, managementPeriod);
                }
            }
        }

        /// <summary>
        /// Initialize the default emission factors for the <see cref="ManagementPeriod"/>.
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the <see cref="ManagementPeriod"/></param>
        /// <param name="managementPeriod">The <see cref="ManagementPeriod"/> that will be reinitialized to new default values</param>
        public void InitializeDefaultEmissionFactors(
            Farm farm,
            ManagementPeriod managementPeriod)
        {
            if (farm != null &&
                managementPeriod != null)
            {
                var emissionData = _livestockEmissionConversionFactorsProvider.GetFactors(manureStateType: managementPeriod.ManureDetails.StateType,
                    meanAnnualPrecipitation: farm.ClimateData.PrecipitationData.GetTotalAnnualPrecipitation(),
                    meanAnnualTemperature: farm.ClimateData.TemperatureData.GetMeanAnnualTemperature(),
                    meanAnnualEvapotranspiration: farm.ClimateData.EvapotranspirationData.GetTotalAnnualEvapotranspiration(),
                    beddingRate: managementPeriod.HousingDetails.UserDefinedBeddingRate,
                    animalType: managementPeriod.AnimalType,
                    farm: farm,
                    year: managementPeriod.Start.Date.Year);

                managementPeriod.ManureDetails.MethaneConversionFactor = emissionData.MethaneConversionFactor;
                managementPeriod.ManureDetails.N2ODirectEmissionFactor = emissionData.N20DirectEmissionFactor;
                managementPeriod.ManureDetails.VolatilizationFraction = emissionData.VolatilizationFraction;
                managementPeriod.ManureDetails.EmissionFactorVolatilization = emissionData.EmissionFactorVolatilization;
                managementPeriod.ManureDetails.EmissionFactorLeaching = emissionData.EmissionFactorLeach;
            }
        }

        /// <summary>
        /// Reinitialize the <see cref="Table_30_Default_Bedding_Material_Composition_Data"/> for all <see cref="ManagementPeriod"/>s for the <see cref="Farm"/>.
        /// </summary>
        /// <param name="farm">The farm containing the <see cref="ManagementPeriod"/>s to initialize with new defaults</param>
        public void ReinitializeBeddingMaterial(Farm farm)
        {
            if (farm != null)
            {
                var data = _beddingMaterialCompositionProvider.Data;
                farm.DefaultsCompositionOfBeddingMaterials.Clear();
                farm.DefaultsCompositionOfBeddingMaterials.AddRange(data);

                foreach (var managementPeriod in farm.GetAllManagementPeriods())
                {
                    var beddingMaterialComposition = farm.GetBeddingMaterialComposition(
                        beddingMaterialType: managementPeriod.HousingDetails.BeddingMaterialType,
                        animalType: managementPeriod.AnimalType);

                    this.InitializeBeddingMaterial(managementPeriod, beddingMaterialComposition);
                }
            }
        }

        /// <summary>
        /// Reinitialize the <see cref="Table_30_Default_Bedding_Material_Composition_Data"/> for the <see cref="ManagementPeriod"/>.
        /// </summary>
        /// <param name="managementPeriod">The <see cref="ManagementPeriod"/> that will be reinitialized to new default values</param>
        /// <param name="data">The defaults to use for the initialization</param>
        public void InitializeBeddingMaterial(ManagementPeriod managementPeriod, Table_30_Default_Bedding_Material_Composition_Data data)
        {
            if (managementPeriod != null && managementPeriod.HousingDetails != null && data != null)
            {
                managementPeriod.HousingDetails.TotalCarbonKilogramsDryMatterForBedding = data.TotalCarbonKilogramsDryMatter;
                managementPeriod.HousingDetails.TotalNitrogenKilogramsDryMatterForBedding = data.TotalNitrogenKilogramsDryMatter;
                managementPeriod.HousingDetails.TotalPhosphorusKilogramsDryMatterForBedding = data.TotalPhosphorusKilogramsDryMatter;
                managementPeriod.HousingDetails.MoistureContentOfBeddingMaterial = data.MoistureContent;
            }
        }

        /// <summary>
        /// Reinitialize the MilkProduction value for each ManagementPeriod for each animalGroup in the DairyComponent of a <see cref="Farm"/> with new default values from table 21.
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> that will be reinitialized to new default value for the MilkProduction</param>
        public void InitializeMilkProduction(Farm farm)
        {
            if (farm != null && farm.DairyComponents != null)
            {
                foreach (var dairyComponent in farm.DairyComponents.Cast<DairyComponent>())
                {
                    if (dairyComponent.Groups != null)
                    {
                        foreach (var animalGroup in dairyComponent.Groups)
                        {
                            if (animalGroup != null && animalGroup.GroupType == AnimalType.DairyLactatingCow)
                            {
                                foreach (var animalGroupManagementPeriod in animalGroup.ManagementPeriods)
                                {
                                    //Calling ReadFile() to get the milkProductionList, extract the province and year to pull value from table and reset to default.
                                    int year = animalGroupManagementPeriod.Start.Year;
                                    IEnumerable<double> milkProduction
                                        = from mp in _milkProductionDataList
                                          where (mp.Province == farm.DefaultSoilData.Province && (int)mp.Year == year)
                                          select mp.AverageMilkProduction;
                                    if (milkProduction?.Any() != true)
                                    {
                                        throw new NullReferenceException();
                                    }
                                    else
                                    {
                                        animalGroupManagementPeriod.MilkProduction = milkProduction.First();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Reinitialize the Beef_Dairy_Cattle_Feeding_Activity_Coefficient object
        /// </summary>
        /// <param name="farm"> Contains the ActivityCoefficientFeedingSituation of each HousingDetails of each ManagementPeriod of each <see cref="farm"/></param>
        public void InitializeCattleFeedingActivity(Farm farm)
        {
            if (farm != null)
            {
                foreach (var animalComponent in farm.AnimalComponents)
                {
                    foreach (var animalGroup in animalComponent.Groups)
                    {
                        if (animalGroup.GroupType.IsBeefCattleType())
                        {
                            foreach (var managementPeriod in animalGroup.ManagementPeriods)
                            {
                                managementPeriod.HousingDetails.ActivityCeofficientOfFeedingSituation = _beefDairyCattleFeedingActivityCoefficientProvider.GetByHousing(managementPeriod
                                    .HousingDetails.HousingType).FeedingActivityCoefficient;
                            }
                        }
                    }
                }
            }
        }

        public void InitializeParameterAdjustmenstForManure(Farm farm)
        {
            if (farm != null)
            {
                foreach (var animalComponent in farm.AnimalComponents)
                {
                    foreach (var animalGroup in animalComponent.Groups)
                    {
                        foreach (var managementPeriod in animalGroup.ManagementPeriods)
                        {


                        }
                    }
                }
            }
        }
        /// <summary>
        /// Reinitializes the values for Ewes, ram and weanedLamb from Table 22
        /// </summary>
        /// <param name="farm">Contains the coefficient(s), weight(s) and wool production details that need to be reinitialized</param>
        public void InitializeLivestockCoefficientSheep(Farm farm)
        {
            if (farm != null)
            {
                foreach (var managementPeriod in farm.GetAllManagementPeriods())
                {
            
                    var result =
                        _sheepProvider.GetCoefficientsByAnimalType(managementPeriod.AnimalType) as
                            Table_22_Livestock_Coefficients_Sheep_Data;
                    if (result != null)
                    {
                        managementPeriod.HousingDetails.BaselineMaintenanceCoefficient =
                            result.BaselineMaintenanceCoefficient;
                        managementPeriod.WoolProduction = result.WoolProduction;
                        managementPeriod.GainCoefficientA = result.CoefficientA;
                        managementPeriod.GainCoefficientB = result.CoefficientB;
                        managementPeriod.StartWeight = result.DefaultInitialWeight;
                        managementPeriod.EndWeight = result.DefaultFinalWeight;
                    }
                    
                }
            }
        }

        /// <summary>
        /// Initialize all <see cref="ClimateData.BarnTemperatureData"/> for all <see cref="ManagementPeriod"/>s in the <see cref="Farm"/>.
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> to initialize with new defaults</param>
        public void InitializeBarnTemperature(Farm farm)
        {
            if (farm != null && farm.ClimateData != null)
            {
                var climateData = farm.ClimateData;
                var province = farm.DefaultSoilData.Province;

                climateData.BarnTemperatureData = _indoorTemperatureProvider.GetIndoorTemperature(province);
                climateData.BarnTemperatureData.IsInitialized = true;
            }
        }

        public void InitializeCropViewItems(Farm farm)
        {
            if (farm != null)
            {
                foreach (var fieldSystemComponent in farm.FieldSystemComponents)
                {
                    foreach (var cropViewItem in fieldSystemComponent.CropViewItems)
                    {
                        this.InitializePercentageReturns(farm, cropViewItem);
                    }   
                }
            }
        }

        public void InitializeCropViewItem(Farm farm, CropViewItem viewItem)
        {
            this.InitializePercentageReturns(farm, viewItem);
        }

        /// <summary>
        /// Initialize default percentage return to soil values for a <see cref="H.Core.Models.LandManagement.Fields.CropViewItem"/>.
        /// </summary>
        /// <param name="farm">The farm containing the <see cref="Defaults"/> object used to initialize each <see cref="CropViewItem"/></param>
        /// <param name="viewItem">The <see cref="CropViewItem"/> that will be initialized</param>
        public void InitializePercentageReturns(Farm farm, CropViewItem viewItem)
        {
            if (farm != null && viewItem != null)
            {
                var defaults = farm.Defaults;

                /*
                 * Initialize the view item by checking the crop type
                 */

                if (viewItem.CropType.IsPerennial())
                {
                    viewItem.PercentageOfProductYieldReturnedToSoil = defaults.PercentageOfProductReturnedToSoilForPerennials;
                    viewItem.PercentageOfStrawReturnedToSoil = 0;
                    viewItem.PercentageOfRootsReturnedToSoil = defaults.PercentageOfRootsReturnedToSoilForPerennials;
                }
                else if (viewItem.CropType.IsAnnual())
                {
                    viewItem.PercentageOfProductYieldReturnedToSoil = defaults.PercentageOfProductReturnedToSoilForAnnuals;
                    viewItem.PercentageOfRootsReturnedToSoil = defaults.PercentageOfRootsReturnedToSoilForAnnuals;
                    viewItem.PercentageOfStrawReturnedToSoil = defaults.PercentageOfStrawReturnedToSoilForAnnuals;
                }

                if (viewItem.CropType.IsRootCrop())
                {
                    viewItem.PercentageOfProductYieldReturnedToSoil = defaults.PercentageOfProductReturnedToSoilForRootCrops;
                    viewItem.PercentageOfStrawReturnedToSoil = defaults.PercentageOfStrawReturnedToSoilForRootCrops;
                }

                if (viewItem.CropType.IsCoverCrop())
                {
                    viewItem.PercentageOfProductYieldReturnedToSoil = 100;
                    viewItem.PercentageOfStrawReturnedToSoil = 100;
                    viewItem.PercentageOfRootsReturnedToSoil = 100;
                }

                /*
                 * Initialize the view item by checking the harvest method (override any setting based on crop type
                 */

                if (viewItem.CropType.IsSilageCrop() || viewItem.HarvestMethod == HarvestMethods.Silage)
                {
                    viewItem.PercentageOfProductYieldReturnedToSoil = 2;
                    viewItem.PercentageOfStrawReturnedToSoil = 0;
                    viewItem.PercentageOfRootsReturnedToSoil = 100;
                }
                else if (viewItem.HarvestMethod == HarvestMethods.Swathing)
                {
                    viewItem.PercentageOfProductYieldReturnedToSoil = 30;
                    viewItem.PercentageOfStrawReturnedToSoil = 0;
                    viewItem.PercentageOfRootsReturnedToSoil = 100;
                }
                else if (viewItem.HarvestMethod == HarvestMethods.GreenManure)
                {
                    viewItem.PercentageOfProductYieldReturnedToSoil = 100;
                    viewItem.PercentageOfStrawReturnedToSoil = 0;
                    viewItem.PercentageOfRootsReturnedToSoil = 100;
                }
            }
        }

        /// <summary>
        /// Initialize the carbon concentration of each <see cref="CropViewItem"/> within a farm
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the <see cref="CropViewItem"/>s that will have reset their carbon concentrations reset</param>
        public void InitializeCarbonConcentration(Farm farm)
        {
            var defaults = new Defaults();
            var viewItems = farm.GetCropDetailViewItems();
            foreach (var viewItem in viewItems)
            {
                InitializeCarbonConcentration(viewItem, defaults);
            }
        }

        /// <summary>
        /// Initialize the carbon concentration of the <see cref="CropViewItem"/> with the values in the <see cref="Defaults"/> paramaeter
        /// </summary>
        /// <param name="viewItem">The <see cref="CropViewItem"/> to have it's carbon concentration reset with default value</param>
        /// <param name="defaults">The <see cref="Defaults"/> containing the default carbon concentration</param>
        public void InitializeCarbonConcentration(CropViewItem viewItem, Defaults defaults)
        {
            viewItem.CarbonConcentration = defaults.CarbonConcentration;
        }
        /// <summary>
        /// Initialize the nitrogen fixation for each <see cref="CropViewItem"/> within a <see cref="Farm"/>
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the <see cref="CropViewItem"/>'s that will have their nitrogen fixation reinitialized</param>
        public void InitializeNitrogenFixation(Farm farm)
        {
            var viewItems = farm.GetCropDetailViewItems();
            foreach (var viewItem in viewItems)
            {
                InitializeNitrogenFixation(viewItem);
            }
        }
        /// <summary>
        /// Initialize the nitrogen fixation within a <see cref="CropViewItem"/>
        /// </summary>
        /// <param name="viewItem">The <see cref="CropViewItem"/> having its nitrogen fixation reinitialized</param>
        public void InitializeNitrogenFixation(CropViewItem viewItem)
        {
            viewItem.NitrogenFixationPercentage =
                _nitrogenFixationProvider.GetNitrogenFixationResult(viewItem.CropType).Fixation * 100;
        }

        /// <summary>
        /// Sets default moisture percentage to the cropViewItem component.
        /// </summary>
        /// <param name="farm"></param>
        public void InitializeMoistureContent(Farm farm)
        {
            if (farm != null)
            {
                foreach (var stateStage in farm.StageStates.OfType<FieldSystemDetailsStageState>().ToList())
                {
                    foreach (var cropViewItem in stateStage.DetailsScreenViewCropViewItems)
                    {
                        var soilFunctionCategory = farm.GetPreferredSoilData(cropViewItem).SoilFunctionalCategory;
                        var residueData = _relativeBiomassInformationProvider.GetResidueData(cropViewItem.IrrigationType,
                            cropViewItem.AmountOfIrrigation, cropViewItem.CropType, soilFunctionCategory, farm.Province);
                       
                        this.InitializeMoistureContent(residueData, cropViewItem);
                    }
                }
            }
        }
        /// <summary>
        /// Sets default moisture percentage to the cropViewItem component.
        /// </summary>
        /// <param name="residueData"> Contains the <see cref="Table_7_Relative_Biomass_Information_Data"/> data to help initialize the <see cref="CropViewItem"/></param>
        /// <param name="cropViewItem"> Contains the <see cref="CropViewItem"/> value to be changed</param>
        public void InitializeMoistureContent(
            Table_7_Relative_Biomass_Information_Data residueData, CropViewItem cropViewItem)
        {
            if (cropViewItem.HarvestMethod == HarvestMethods.GreenManure ||
                cropViewItem.HarvestMethod == HarvestMethods.Silage ||
                cropViewItem.HarvestMethod == HarvestMethods.Swathing ||
                cropViewItem.CropType.IsSilageCrop())
            {
                cropViewItem.MoistureContentOfCropPercentage = 65;
            }
            else
            {
                if (residueData != null && residueData.MoistureContentOfProduct != 0)
                {
                    cropViewItem.MoistureContentOfCropPercentage = residueData.MoistureContentOfProduct;
                }
                else
                {
                    cropViewItem.MoistureContentOfCropPercentage = 12;
                }
            }
        }

        public void InitializeBiomassCoefficients(CropViewItem viewItem, Farm farm)
        {
            var soilFunctionCategory = farm.GetPreferredSoilData(viewItem).SoilFunctionalCategory;
            var residueData = _relativeBiomassInformationProvider.GetResidueData(viewItem.IrrigationType, viewItem.AmountOfIrrigation, viewItem.CropType, soilFunctionCategory, farm.Province);
            if (residueData != null)
            {
                viewItem.BiomassCoefficientProduct = residueData.RelativeBiomassProduct;
                viewItem.BiomassCoefficientStraw = residueData.RelativeBiomassStraw;
                viewItem.BiomassCoefficientRoots = residueData.RelativeBiomassRoot;
                viewItem.BiomassCoefficientExtraroot = residueData.RelativeBiomassExtraroot;

                if (viewItem.HarvestMethod == HarvestMethods.Swathing || viewItem.HarvestMethod == HarvestMethods.GreenManure || viewItem.HarvestMethod == HarvestMethods.Silage)
                {
                    viewItem.BiomassCoefficientProduct = residueData.RelativeBiomassProduct + residueData.RelativeBiomassStraw;
                    viewItem.BiomassCoefficientStraw = 0;
                    viewItem.BiomassCoefficientRoots = residueData.RelativeBiomassRoot;
                    viewItem.BiomassCoefficientExtraroot = residueData.RelativeBiomassExtraroot;
                }
            }
        }

        public void InitializeNitrogenContent(CropViewItem viewItem, Farm farm)
        {
            // Assign N content values used for the ICBM methodology
            var soilFunctionCategory = farm.GetPreferredSoilData(viewItem).SoilFunctionalCategory;
            var residueData = _relativeBiomassInformationProvider.GetResidueData(viewItem.IrrigationType, viewItem.AmountOfIrrigation, viewItem.CropType, soilFunctionCategory, farm.Province);
            if (residueData != null)
            {
                // Table has values in grams but unit of display is kg
                viewItem.NitrogenContentInProduct = residueData.NitrogenContentProduct / 1000;
                viewItem.NitrogenContentInStraw = residueData.NitrogenContentStraw / 1000;
                viewItem.NitrogenContentInRoots = residueData.NitrogenContentRoot / 1000;
                viewItem.NitrogenContentInExtraroot = residueData.NitrogenContentExtraroot / 1000;

                if (viewItem.CropType.IsPerennial())
                {
                    viewItem.NitrogenContentInStraw = 0;
                }
            }

            // Assign N content values used for IPCC Tier 2
            var cropData = _slopeProviderTable.GetDataByCropType(viewItem.CropType);
            
            viewItem.NitrogenContent = cropData.NitrogenContentResidues;
        }

        public void InitializeMoistureContent(CropViewItem viewItem, Farm farm)
        {
            var soilFunctionCategory = farm.GetPreferredSoilData(viewItem).SoilFunctionalCategory;
            var residueData = _relativeBiomassInformationProvider.GetResidueData(viewItem.IrrigationType,
                viewItem.AmountOfIrrigation, viewItem.CropType, soilFunctionCategory, farm.Province);

            this.InitializeMoistureContent(residueData, viewItem);
        }

        /// <summary>
        /// Sets the tillage type for a view item based on the province.
        /// </summary>
        public void InitializeTillageType(
            CropViewItem viewItem,
            Farm farm)
        {
            var province = farm.DefaultSoilData.Province;
            var soilFunctionCategory = farm.GetPreferredSoilData(viewItem).SoilFunctionalCategory;
            var residueData = _relativeBiomassInformationProvider.GetResidueData(viewItem.IrrigationType, viewItem.AmountOfIrrigation, viewItem.CropType, soilFunctionCategory, province);

            if (residueData != null)
            {
                if (residueData.TillageTypeTable.ContainsKey(province))
                {
                    var tillageTypeForProvince = residueData.TillageTypeTable[province];

                    viewItem.TillageType = tillageTypeForProvince;
                    viewItem.PastTillageType = tillageTypeForProvince;
                }
            }

            if (viewItem.CropType.IsPerennial())
            {
                viewItem.TillageType = TillageType.NoTill;
                viewItem.PastTillageType = TillageType.NoTill;
            }
        }

        public void InitializeFallow(CropViewItem viewItem, Farm farm)
        {
            if (viewItem.CropType.IsFallow())
            {
                viewItem.Yield = 0;
                viewItem.TillageType = farm.Defaults.DefaultTillageTypeForFallow;
                viewItem.PastTillageType = TillageType.NoTill;
                viewItem.HarvestMethod = HarvestMethods.None;
                viewItem.PercentageOfProductYieldReturnedToSoil = 0;
                viewItem.PercentageOfStrawReturnedToSoil = 0;
                viewItem.PercentageOfRootsReturnedToSoil = 0;
            }
        }

        public void InitializeHarvestMethod(CropViewItem viewItem, Farm farm)
        {
            if (viewItem.CropType.IsSilageCrop())
            {
                viewItem.HarvestMethod = HarvestMethods.Silage;
            }
            else
            {
                viewItem.HarvestMethod = HarvestMethods.CashCrop;
            }
        }

        public void InitializePerennialDefaults(CropViewItem viewItem, Farm farm)
        {
            if (viewItem.CropType.IsPerennial())
            {
                viewItem.TillageType = TillageType.NoTill;
                viewItem.PastTillageType = TillageType.NoTill;
                viewItem.FertilizerApplicationMethodology = FertilizerApplicationMethodologies.Broadcast;
                viewItem.ForageUtilizationRate = _utilizationRatesForLivestockGrazingProvider.GetUtilizationRate(viewItem.CropType);
                viewItem.TotalBiomassHarvest = viewItem.DefaultYield;
            }
        }

        #endregion
    }
}