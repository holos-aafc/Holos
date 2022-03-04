using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
using AutoMapper;
using H.Core.Calculators.Carbon;
using H.Core.Calculators.Climate;
using H.Core.Calculators.Economics;
using H.Core.Calculators.Nitrogen;
using H.Core.Calculators.Tillage;
using H.Core.Calculators.UnitsOfMeasurement;
using H.Core.Emissions;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Models.Results;
using H.Core.Providers.Animals;
using H.Core.Providers.Carbon;
using H.Core.Providers.Economics;
using H.Core.Providers.Fertilizer;
using H.Core.Providers.Nitrogen;
using H.Core.Providers.Plants;
using H.Core.Providers.Soil;
using H.Core.Tools;
using H.Infrastructure;

namespace H.Core.Services.LandManagement
{
    public partial class FieldResultsService : IFieldResultsService
    {
        #region Fields

        private const int DefaultNumberOfDecimalPlaces = 3;

        private readonly IClimateParameterCalculator _climateParameterCalculator = new ClimateParameterCalculator();
        private readonly SoilEmissionsCalculator _soilEmissionsCalculator = new SoilEmissionsCalculator();
        private readonly IICBMSoilCarbonCalculator _icbmSoilCarbonCalculator = new ICBMSoilCarbonCalculator();
        private readonly IPCCTier2SoilCarbonCalculator _tier2SoilCarbonCalculator = new IPCCTier2SoilCarbonCalculator();
        private readonly ITillageFactorCalculator _tillageFactorCalculator = new TillageFactorCalculator();
        private readonly EnergyCarbonDioxideEmissionsCalculator _energyCarbonDioxideEmissionsCalculator = new EnergyCarbonDioxideEmissionsCalculator();
        private readonly MultiYearNitrousOxideCalculator _multiYearNitrousOxideCalculator = new MultiYearNitrousOxideCalculator();
        private readonly UnitsOfMeasurementCalculator _unitsCalculator = new UnitsOfMeasurementCalculator();
        private readonly SingleYearNitrousOxideCalculator _singleYearNitrogenEmissionsCalculator = new SingleYearNitrousOxideCalculator();

        private readonly LandManagementChangeHelper _landManagementChangeHelper = new LandManagementChangeHelper();
        private readonly EconomicsHelper _economicsHelper = new EconomicsHelper();

        private readonly IMapper _detailViewItemMapper;
        private readonly IMapper _manureApplicationViewItemMapper;

        private readonly FertilizerBlendProvider _fertilizerBlendProvider = new FertilizerBlendProvider();
        private readonly NitrogenLigninInCropsForSteadyStateMethodProvider_Table_12 _slopeProviderTable = new NitrogenLigninInCropsForSteadyStateMethodProvider_Table_12();
        private readonly LumCMaxAndkValueForPerennialsAndGrasslandProvider_Table_3 _lumCMaxAndkValueForPerennialsAndGrasslandProviderTable3 = new LumCMaxAndkValueForPerennialsAndGrasslandProvider_Table_3();
        private readonly LumCMaxAndKValuesForTillagePracticeChangeProvider_Table_1 _lumCMaxAndKValuesForTillagePracticeChangeProviderTable1 = new LumCMaxAndKValuesForTillagePracticeChangeProvider_Table_1();
        private readonly LumCMaxAndKValuesForFallowPracticeChangeProvider_Table_2 _lumCMaxAndKValuesForFallowPracticeChangeProviderTable2 = new LumCMaxAndKValuesForFallowPracticeChangeProvider_Table_2();
        private readonly SoilNitrousOxideEmissionFactorProvider_Table_15 _soilNitrousOxideEmissionFactorProvider = new SoilNitrousOxideEmissionFactorProvider_Table_15();
        private readonly DefaultManureCompositionProvider_Table_9 _defaultManureCompositionProvider = new DefaultManureCompositionProvider_Table_9();
        private readonly CanadianAgriculturalRegionIdToSlcIdProvider _canadianAgriculturalRegionIdToSlcIdProvider = new CanadianAgriculturalRegionIdToSlcIdProvider();
        private readonly SmallAreaYieldProvider _smallAreaYieldProvider = new SmallAreaYieldProvider();
        private readonly EnergyRequirementsForCropsProvider_Table_38 _energyRequirementsForCropsProviderTable38 = new EnergyRequirementsForCropsProvider_Table_38();
        private readonly DefaultAmmoniaEmissionProvider_Table_39 _defaultAmmoniaEmissionFactorProvider = new DefaultAmmoniaEmissionProvider_Table_39();
        private readonly EcodistrictDefaultsProvider _ecodistrictDefaultsProvider = new EcodistrictDefaultsProvider();
        private readonly NitogenFixationProvider _nitrogenFixationProvider = new NitogenFixationProvider();
        private readonly ForageUtilizationRateProvider _forageUtilizationRateProvider = new ForageUtilizationRateProvider();
        private readonly ICustomFileYieldProvider _customFileYieldProvider = new CustomFileYieldProvider();
        private readonly ResidueDataProvider _residueDataProvider = new ResidueDataProvider();
        private readonly CropEconomicsProvider _economicsProvider = new CropEconomicsProvider();

        private readonly Dictionary<FieldSystemComponent, FieldComponentEmissionResults> _fieldComponentEmissionResultsCache = new Dictionary<FieldSystemComponent, FieldComponentEmissionResults>();

        #endregion

        #region Constructors

        public FieldResultsService()
        {
            HTraceListener.AddTraceListener();

            /*
             * Create a mapper that will map component selection view items to detail view items
             */

            var componentSelectionViewItemToDetailViewItemMapperConfiguration = new MapperConfiguration(configuration =>
            {
                configuration.CreateMap<CropViewItem, CropViewItem>()
                    .ForMember(property => property.Name, options => options.Ignore())
                    .ForMember(property => property.Guid, options => options.Ignore())
                    .ForMember(property => property.HarvestViewItems, options => options.Ignore())
                    .ForMember(property => property.GrazingViewItems, options => options.Ignore())
                    .ForMember(property => property.ManureApplicationViewItems, options => options.Ignore());
            });

            _detailViewItemMapper = componentSelectionViewItemToDetailViewItemMapperConfiguration.CreateMapper();

            var manureApplicationViewItemConfiguration = new MapperConfiguration(configure: configuration =>
            {
                configuration.CreateMap<ManureApplicationViewItem, ManureApplicationViewItem>()
                    .ForMember(property => property.Name, options => options.Ignore())
                    .ForMember(property => property.Guid, options => options.Ignore());
            });

            _manureApplicationViewItemMapper = manureApplicationViewItemConfiguration.CreateMapper();

            _smallAreaYieldProvider.InitializeAsync();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Calculate final results for all fields on the farm
        /// </summary>
        public List<FieldComponentEmissionResults> CalculateResultsForFieldComponent(Farm farm)
        {
            var result = new List<FieldComponentEmissionResults>();

            foreach (var farmFieldSystemComponent in farm.FieldSystemComponents)
            {
                var emissions = this.CalculateResultsForFieldComponent(farmFieldSystemComponent, farm);
                result.Add(emissions);
            }

            return result;
        }

        /// <summary>
        /// Calculate final results for one field
        /// </summary>
        public FieldComponentEmissionResults CalculateResultsForFieldComponent(
            FieldSystemComponent fieldSystemComponent, 
            Farm farm)
        {
            var results = new FieldComponentEmissionResults();

            // Check if the results have been calculated for this field already
            if (fieldSystemComponent.ResultsCalculated)
            {
                // Check if we cached the results already
                if (_fieldComponentEmissionResultsCache.ContainsKey(fieldSystemComponent))
                {
                    Trace.TraceInformation($"{nameof(FieldResultsService)}.{nameof(FieldResultsService.CalculateResultsForFieldComponent)}: results already calculated for {fieldSystemComponent.Name}, returning cached results.");

                    return _fieldComponentEmissionResultsCache[fieldSystemComponent];
                }

                // Results are calculated but not cached yet (system boot), calculate results for this field now
            }

            Trace.TraceInformation($"{nameof(FieldResultsService)}.{nameof(FieldResultsService.CalculateResultsForFieldComponent)}: calculating results for field: '{fieldSystemComponent.Name}'");

            results.LandUseChangeResults = this.CalculateLandUseChangeResults(fieldSystemComponent, farm);
            results.CropEnergyResults = this.CalculateCropEnergyResults(fieldSystemComponent, farm);
            results.CropN2OEmissionsResults = this.CalculateCropN2OEmissions(fieldSystemComponent, farm);
            results.HarvestViewItems.AddRange(this.CalculateHarvestForField(fieldSystemComponent, farm));

            results.FieldSystemComponent = fieldSystemComponent;
            results.Name = fieldSystemComponent.Name + " - " + fieldSystemComponent.CropString;

            fieldSystemComponent.ResultsCalculated = true;

            _fieldComponentEmissionResultsCache[fieldSystemComponent] = results;

            return results;
        }

        /// <summary>
        /// Calculates final multiyear C and N2O results for a collection of farms
        /// </summary>
        public List<CropViewItem> CalculateFinalResults(IEnumerable<Farm> farms)
        {
            var results = new List<CropViewItem>();

            foreach (var farm in farms)
            {
                var result = this.CalculateFinalResults(farm);
                results.AddRange(result);
            }

            return results;
        }

        /// <summary>
        /// Calculates final multiyear C and N2O results for a farm
        /// </summary>
        public List<CropViewItem> CalculateFinalResults(Farm farm)
        {
            var result = new List<CropViewItem>();

            // Get all of the detail view items for all fields for this farm
            var detailsStageState = this.GetStageState(farm);
            if (detailsStageState != null)
            {
                /*
                 * Group all detail view items by field GUID, then create a result view item for each. This is required since the stage state will hold
                 * detail view items for all fields on a farm
                 */

                var viewItemsGroupedByField =  detailsStageState.DetailsScreenViewCropViewItems.GroupBy(viewItem => viewItem.FieldSystemComponentGuid);
                foreach (var groupingByFieldSystem in viewItemsGroupedByField)
                {
                    var fieldGuid = groupingByFieldSystem.Key;
                    var fieldSystemComponent = farm.GetFieldSystemComponent(fieldGuid);

                    var detailViewItems = groupingByFieldSystem.ToList();

                    /*
                     * At this point there could be multiple items for one year (e.g. a main crop and a cover crop or an undersown crop), here we combine
                     * multiple inputs from same year into the main crop
                     */
                    this.CombineInputsForAllCropsInSameYear(detailViewItems, fieldSystemComponent);

                    // Merge multiple items with the same year into a single year view items so that no two view items have the same year when calculating ICBM results (ICBM calculations
                    // require exactly one item per year (with combined inputs when there is a secondary crop grown)
                    var mergedItems = this.MergeDetailViewItems(detailViewItems, fieldSystemComponent);

                    this.CalculateFinalResultsForField(
                        viewItemsForField: mergedItems, 
                        farm: farm, 
                        fieldSystemGuid: groupingByFieldSystem.Key);

                    // Add the merged items for this field into the results collection

                    if (farm.Defaults.CarbonModellingStrategy == CarbonModellingStrategies.IPCCTier2 && farm.Defaults.OutputRunInPeriodItems == false)
                    {
                        // Remove the run in period items
                        result.AddRange(mergedItems.Where(x => x.Year >= fieldSystemComponent.StartYear));
                    }
                    else
                    {
                        result.AddRange(mergedItems);
                    }
                }

                this.CalculateAverageSoilOrganicCarbonForFields(result);
            }

            return result;
        }

        /// <summary>
        /// Calculate climate parameter. Will use custom climate data if it exists for the farm, otherwise will use SLC normals
        /// for climate data.
        /// </summary>
        public double CalculateClimateParameter(CropViewItem viewItem, Farm farm)
        {
            if (farm.ClimateData.DailyClimateData.Any())
            {
                var climateDataGroupedByYear = farm.ClimateData.DailyClimateData.GroupBy(userClimateData => userClimateData.Year);
                var climateDataForYear = climateDataGroupedByYear.SingleOrDefault(groupingByYear => groupingByYear.Key == viewItem.Year);
                var climateParameter = 0d;

                if (climateDataForYear != null)
                {
                    // Use daily climate data

                    var precipitationList = climateDataForYear.OrderBy(climateData => climateData.JulianDay).Select(climateData => climateData.MeanDailyPrecipitation).ToList();
                    var temperatureList = climateDataForYear.OrderBy(climateData => climateData.JulianDay).Select(climateData => climateData.MeanDailyAirTemperature).ToList();
                    var evapotranspirationList = climateDataForYear.OrderBy(climateData => climateData.JulianDay).Select(climateData => climateData.MeanDailyPET).ToList();

                    climateParameter = _climateParameterCalculator.CalculateClimateParameterForYear(
                        farm: farm,
                        cropViewItem: viewItem,
                        evapotranspirations: evapotranspirationList,
                        precipitations: precipitationList,
                        temperatures: temperatureList);
                }
                else
                {
                    // If user has entered custom climate data but their input file has no data for a particular year, then use normals for that particular year

                    climateParameter = _climateParameterCalculator.CalculateClimateParameterForYear(
                        farm: farm,
                        cropViewItem: viewItem,
                        evapotranspirations: farm.ClimateData.EvapotranspirationData.GetAveragedYearlyValues(),
                        precipitations: farm.ClimateData.PrecipitationData.GetAveragedYearlyValues(),
                        temperatures: farm.ClimateData.TemperatureData.GetAveragedYearlyValues());
                }

                return Math.Round(climateParameter, DefaultNumberOfDecimalPlaces);
            }
            else
            {
                // Use SLC normals when there is no custom user climate data

                Trace.TraceWarning($"{nameof(FieldResultsService)}: No custom daily climate data exists for this farm. Defaulting to SLC climate normals (and averaged daily values)");

                var result = _climateParameterCalculator.CalculateClimateParameterForYear(
                    farm: farm,
                    cropViewItem: viewItem,
                    evapotranspirations: farm.ClimateData.EvapotranspirationData.GetAveragedYearlyValues(),
                    precipitations: farm.ClimateData.PrecipitationData.GetAveragedYearlyValues(),
                    temperatures: farm.ClimateData.TemperatureData.GetAveragedYearlyValues());

                return Math.Round(result, DefaultNumberOfDecimalPlaces);
            }
        }

        public double CalculateTillageFactor(CropViewItem viewItem, Farm farm)
        {
            var result = _tillageFactorCalculator.CalculateTillageFactor(
                province: farm.Province,
                soilFunctionalCategory: farm.GeographicData.DefaultSoilData.SoilFunctionalCategory,
                tillageType: viewItem.TillageType,
                cropType: viewItem.CropType,
                perennialYear: viewItem.YearInPerennialStand);

            return Math.Round(result, DefaultNumberOfDecimalPlaces);
        }

        public double CalculateManagementFactor(double climateParameter, double tillageFactor)
        {
            var result = _climateParameterCalculator.CalculateClimateManagementFactor(climateParameter, tillageFactor);

            return Math.Round(result, DefaultNumberOfDecimalPlaces);
        }

        public ResidueData GetResidueData(CropViewItem cropViewItem, Farm farm)
        {
            var province = farm.Province;
            var geographicData = farm.GeographicData;
            var soilData = geographicData.DefaultSoilData;
            if (soilData == null)
            {
                return new ResidueData();
            }

            var residueData = _residueDataProvider.GetResidueData(
                irrigationType: cropViewItem.IrrigationType,
                irrigationAmount: cropViewItem.AmountOfIrrigation,
                cropType: cropViewItem.CropType,
                soilFunctionalCategory: soilData.SoilFunctionalCategory,
                province: province);

            return residueData;
        }

        #endregion

        #region Private Methods
        #endregion
    }
}
