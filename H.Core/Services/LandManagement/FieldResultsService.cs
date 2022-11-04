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
using H.Core.Providers.Energy;
using H.Core.Providers.Fertilizer;
using H.Core.Providers.Nitrogen;
using H.Core.Providers.Plants;
using H.Core.Providers.Soil;
using H.Core.Services.Animals;
using H.Core.Tools;
using H.Infrastructure;

namespace H.Core.Services.LandManagement
{
    public partial class FieldResultsService : IFieldResultsService
    {
        #region Fields

        private const int DefaultNumberOfDecimalPlaces = 3;

        private readonly AnimalResultsService _animalResultsService = new AnimalResultsService();
        private readonly IrrigationService _irrigationService = new IrrigationService();

        private readonly IClimateParameterCalculator _climateParameterCalculator = new ClimateParameterCalculator();
        private readonly ICBMSoilCarbonCalculator _icbmSoilCarbonCalculator = new ICBMSoilCarbonCalculator();
        private readonly IPCCTier2SoilCarbonCalculator _tier2SoilCarbonCalculator = new IPCCTier2SoilCarbonCalculator();
        private readonly ITillageFactorCalculator _tillageFactorCalculator = new TillageFactorCalculator();
        private readonly UnitsOfMeasurementCalculator _unitsCalculator = new UnitsOfMeasurementCalculator();
        private readonly SingleYearNitrousOxideCalculator _singleYearNitrogenEmissionsCalculator = new SingleYearNitrousOxideCalculator();

        private readonly LandManagementChangeHelper _landManagementChangeHelper = new LandManagementChangeHelper();
        private readonly EconomicsHelper _economicsHelper = new EconomicsHelper();

        private readonly IMapper _detailViewItemMapper;
        private readonly IMapper _manureApplicationViewItemMapper;
        private readonly IMapper _harvestViewItemMapper;
        private readonly IMapper _hayImportViewItemMapper;

        private readonly Table_51_Carbon_Footprint_For_Fertilizer_Blends_Provider _carbonFootprintForFertilizerBlendsProvider = new Table_51_Carbon_Footprint_For_Fertilizer_Blends_Provider();
        private readonly Table_12_Nitrogen_Lignin_Content_In_Crops_Provider _slopeProviderTable = new Table_12_Nitrogen_Lignin_Content_In_Crops_Provider();
        private readonly Table_5_LumCMax_KValues_Perennial_Cropping_Change_Provider _lumCMaxKValuesPerennialCroppingChangeProvider = new Table_5_LumCMax_KValues_Perennial_Cropping_Change_Provider();
        private readonly Table_3_LumCMax_KValues_Tillage_Practice_Change_Provider _lumCMaxKValuesTillagePracticeChangeProvider = new Table_3_LumCMax_KValues_Tillage_Practice_Change_Provider();
        private readonly Table_4_LumCMax_KValues_Fallow_Practice_Change_Provider _lumCMaxKValuesFallowPracticeChangeProvider = new Table_4_LumCMax_KValues_Fallow_Practice_Change_Provider();
        private readonly SmallAreaYieldProvider _smallAreaYieldProvider = new SmallAreaYieldProvider();
        private readonly Table_53_Fuel_Energy_Estimates_Provider _fuelEnergyEstimatesProvider = new Table_53_Fuel_Energy_Estimates_Provider();
        private readonly Table_54_Herbicide_Energy_Estimates_Provider _herbicideEnergyEstimatesProvider = new Table_54_Herbicide_Energy_Estimates_Provider();
        private readonly EcodistrictDefaultsProvider _ecodistrictDefaultsProvider = new EcodistrictDefaultsProvider();
        private readonly NitogenFixationProvider _nitrogenFixationProvider = new NitogenFixationProvider();
        private readonly Table_71_Utilization_Rates_For_Livestock_Grazing_Provider _utilizationRatesForLivestockGrazingProvider = new Table_71_Utilization_Rates_For_Livestock_Grazing_Provider();
        private readonly ICustomFileYieldProvider _customFileYieldProvider = new CustomFileYieldProvider();
        private readonly Table_10_Relative_Biomass_Provider _relativeBiomassProvider = new Table_10_Relative_Biomass_Provider();
        private readonly CropEconomicsProvider _economicsProvider = new CropEconomicsProvider();

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
                    .ForMember(property => property.HayImportViewItems, options => options.Ignore())
                    .ForMember(property => property.ManureApplicationViewItems, options => options.Ignore());
            });

            _detailViewItemMapper = componentSelectionViewItemToDetailViewItemMapperConfiguration.CreateMapper();

            var manureApplicationViewItemConfiguration = new MapperConfiguration(configure: configuration =>
            {
                configuration.CreateMap<ManureApplicationViewItem, ManureApplicationViewItem>()
                    .ForMember(property => property.Name, options => options.Ignore())
                    .ForMember(property => property.Guid, options => options.Ignore());
            });

            var hayImportViewItemMapperConfiguration = new MapperConfiguration(configure: configuration =>
            {
                configuration.CreateMap<HayImportViewItem, HayImportViewItem>()
                    .ForMember(property => property.Name, options => options.Ignore())
                    .ForMember(property => property.Guid, options => options.Ignore());
            });

            var harvestViewItemMapperConfiguration = new MapperConfiguration(configure: configuration =>
            {
                configuration.CreateMap<HarvestViewItem, HarvestViewItem>()
                    .ForMember(property => property.Name, options => options.Ignore())
                    .ForMember(property => property.Guid, options => options.Ignore());
            });

            _manureApplicationViewItemMapper = manureApplicationViewItemConfiguration.CreateMapper();
            _hayImportViewItemMapper = hayImportViewItemMapperConfiguration.CreateMapper();
            _harvestViewItemMapper = harvestViewItemMapperConfiguration.CreateMapper();

            _smallAreaYieldProvider.InitializeAsync();

            this.AnimalResults = new List<AnimalComponentEmissionsResults>();
        }

        #endregion

        #region Properties

        public List<AnimalComponentEmissionsResults> AnimalResults { get; set; }

        #endregion

        #region Public Methods

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
                    if (fieldSystemComponent == null)
                    {
                        continue;
                    }

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

                    result.AddRange(mergedItems);
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

                    // Add irrigation amounts to daily precipitations
                    var totalPrecipitationList = _irrigationService.AddIrrigationToDailyPrecipitations(precipitationList, farm, viewItem);

                    var temperatureList = climateDataForYear.OrderBy(climateData => climateData.JulianDay).Select(climateData => climateData.MeanDailyAirTemperature).ToList();
                    var evapotranspirationList = climateDataForYear.OrderBy(climateData => climateData.JulianDay).Select(climateData => climateData.MeanDailyPET).ToList();

                    climateParameter = _climateParameterCalculator.CalculateClimateParameterForYear(
                        farm: farm,
                        cropViewItem: viewItem,
                        evapotranspirations: evapotranspirationList,
                        precipitations: totalPrecipitationList,
                        temperatures: temperatureList);
                }
                else
                {
                    // If user has entered custom climate data but their input file has no data for a particular year, then use normals for that particular year
                    
                    // Add irrigation amounts to daily precipitations
                    var totalPrecipitationList = _irrigationService.AddIrrigationToDailyPrecipitations(farm.ClimateData.PrecipitationData.GetAveragedYearlyValues(), farm, viewItem);

                    climateParameter = _climateParameterCalculator.CalculateClimateParameterForYear(
                        farm: farm,
                        cropViewItem: viewItem,
                        evapotranspirations: farm.ClimateData.EvapotranspirationData.GetAveragedYearlyValues(),
                        precipitations: totalPrecipitationList,
                        temperatures: farm.ClimateData.TemperatureData.GetAveragedYearlyValues());
                }

                return Math.Round(climateParameter, DefaultNumberOfDecimalPlaces);
            }
            else
            {
                // Add irrigation amounts to daily precipitations
                var totalPrecipitationList = _irrigationService.AddIrrigationToDailyPrecipitations(farm.ClimateData.PrecipitationData.GetAveragedYearlyValues(), farm, viewItem);

                // Use SLC normals when there is no custom user climate data
                Trace.TraceWarning($"{nameof(FieldResultsService)}: No custom daily climate data exists for this farm. Defaulting to SLC climate normals (and averaged daily values)");

                var result = _climateParameterCalculator.CalculateClimateParameterForYear(
                    farm: farm,
                    cropViewItem: viewItem,
                    evapotranspirations: farm.ClimateData.EvapotranspirationData.GetAveragedYearlyValues(),
                    precipitations: totalPrecipitationList,
                    temperatures: farm.ClimateData.TemperatureData.GetAveragedYearlyValues());

                return Math.Round(result, DefaultNumberOfDecimalPlaces);
            }
        }

        public double CalculateTillageFactor(CropViewItem viewItem, Farm farm)
        {
            var result = _tillageFactorCalculator.CalculateTillageFactor(
                province: farm.DefaultSoilData.Province,
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

        public Table_10_Relative_Biomass_Data GetResidueData(CropViewItem cropViewItem, Farm farm)
        {
            var province = farm.DefaultSoilData.Province;
            var geographicData = farm.GeographicData;
            var soilData = geographicData.DefaultSoilData;
            if (soilData == null)
            {
                return new Table_10_Relative_Biomass_Data();
            }

            var residueData = _relativeBiomassProvider.GetResidueData(
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
