using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
using H.Core.Mappers;
using H.Core.Calculators.Carbon;
using H.Core.Calculators.Climate;
using H.Core.Calculators.Economics;
using H.Core.Calculators.Infrastructure;
using H.Core.Calculators.Nitrogen;
using H.Core.Calculators.Nitrogen.NitrogenService;
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
using H.Core.Services.Initialization;
using H.Core.Tools;
using H.Infrastructure;

namespace H.Core.Services.LandManagement
{
    public partial class FieldResultsService : IFieldResultsService
    {
        #region Fields

        private readonly IClimateService _climateService;

        private readonly IClimateParameterCalculator _climateParameterCalculator = new ClimateParameterCalculator();
        private readonly ICBMSoilCarbonCalculator _icbmSoilCarbonCalculator;
        private readonly IPCCTier2SoilCarbonCalculator _tier2SoilCarbonCalculator;
        private readonly ITillageFactorCalculator _tillageFactorCalculator = new TillageFactorCalculator();
        private readonly UnitsOfMeasurementCalculator _unitsCalculator = new UnitsOfMeasurementCalculator();
        private readonly N2OEmissionFactorCalculator _n2OEmissionFactorCalculator;
        private readonly IManureService _manureService = new ManureService();

        private readonly ModelMapper<CropViewItem> _detailViewItemMapper;
        private readonly ModelMapper<ManureApplicationViewItem> _manureApplicationViewItemMapper;
        private readonly ModelMapper<HarvestViewItem> _harvestViewItemMapper;
        private readonly ModelMapper<GrazingViewItem> _grazingViewItemMapper;
        private readonly ModelMapper<HayImportViewItem> _hayImportViewItemMapper;
        private readonly ModelMapper<FertilizerApplicationViewItem> _fertilizerViewItemMapper;
        private readonly ModelMapper<DigestateApplicationViewItem> _digestateViewItemMapper;

        private readonly Table_48_Carbon_Footprint_For_Fertilizer_Blends_Provider _carbonFootprintForFertilizerBlendsProvider = new Table_48_Carbon_Footprint_For_Fertilizer_Blends_Provider();

        private readonly SmallAreaYieldProvider _smallAreaYieldProvider = new SmallAreaYieldProvider();
        
        private readonly ICustomFileYieldProvider _customFileYieldProvider = new CustomFileYieldProvider();
        private readonly IInitializationService _initializationService;
        private readonly IICBMNitrogenInputCalculator _icbmNitrogenInputCalculator;
        private readonly IIPCCNitrogenInputCalculator _ipccNitrogenInputCalculator;
        private readonly INitrogenService _nitrogenService;
        private readonly ICarbonService _carbonService;
        private readonly IAnimalService _animalService;
        private readonly IFieldComponentHelper _fieldComponentHelper;

        #endregion

        #region Constructors

        public FieldResultsService(
            ICBMSoilCarbonCalculator icbmSoilCarbonCalculator,
            IPCCTier2SoilCarbonCalculator ipccTier2SoilCarbonCalculator,
            N2OEmissionFactorCalculator n2OEmissionFactorCalculator,
            IInitializationService initializationService)
        {
            if (initializationService != null)
            {
                _initializationService = initializationService;
            }
            else
            {
                throw new ArgumentNullException(nameof(initializationService));
            }

            if (icbmSoilCarbonCalculator != null)
            {
                _icbmSoilCarbonCalculator = icbmSoilCarbonCalculator;
            }
            else
            {
                throw new ArgumentNullException(nameof(icbmSoilCarbonCalculator));
            }

            if (ipccTier2SoilCarbonCalculator != null)
            {
                _tier2SoilCarbonCalculator = ipccTier2SoilCarbonCalculator;
            }
            else
            {
                throw new ArgumentNullException(nameof(ipccTier2SoilCarbonCalculator));
            }

            if (n2OEmissionFactorCalculator != null)
            {
                _n2OEmissionFactorCalculator = n2OEmissionFactorCalculator;
            }
            else
            {
                throw new ArgumentNullException(nameof(n2OEmissionFactorCalculator));
            }

            HTraceListener.AddTraceListener();

            /*
             * Create a mapper that will map component selection view items to detail view items
             */

            _detailViewItemMapper = new ModelMapper<CropViewItem>(
                nameof(CropViewItem.Name),
                nameof(CropViewItem.Guid),
                nameof(CropViewItem.HarvestViewItems),
                nameof(CropViewItem.GrazingViewItems),
                nameof(CropViewItem.FertilizerApplicationViewItems),
                nameof(CropViewItem.HayImportViewItems),
                nameof(CropViewItem.DigestateApplicationViewItems),
                nameof(CropViewItem.ManureApplicationViewItems),
                nameof(CropViewItem.MonthlyIpccTier2TemperatureFactors),
                nameof(CropViewItem.MonthlyIpccTier2WaterFactors));

            _manureApplicationViewItemMapper = new ModelMapper<ManureApplicationViewItem>(
                nameof(ManureApplicationViewItem.Name), nameof(ManureApplicationViewItem.Guid));

            _hayImportViewItemMapper = new ModelMapper<HayImportViewItem>(
                nameof(HayImportViewItem.Name), nameof(HayImportViewItem.Guid));

            // Grazing items were previously (incorrectly) cloned via the harvest mapper - a latent bug that only compiled
            // because AutoMapper's IMapper.Map<TSource,TDest> is untyped. Wire the intended grazing mapper.
            _grazingViewItemMapper = new ModelMapper<GrazingViewItem>(
                nameof(GrazingViewItem.Name), nameof(GrazingViewItem.Guid));

            _harvestViewItemMapper = new ModelMapper<HarvestViewItem>(
                nameof(HarvestViewItem.Name), nameof(HarvestViewItem.Guid));

            _fertilizerViewItemMapper = new ModelMapper<FertilizerApplicationViewItem>(
                nameof(FertilizerApplicationViewItem.Name), nameof(FertilizerApplicationViewItem.Guid));

            _digestateViewItemMapper = new ModelMapper<DigestateApplicationViewItem>(
                nameof(DigestateApplicationViewItem.Name), nameof(DigestateApplicationViewItem.Guid));

            _smallAreaYieldProvider.Initialize();

            this.AnimalResults = new List<AnimalComponentEmissionsResults>();

            _nitrogenService = new NitrogenService();
            _carbonService = new CarbonService();
            _animalService = new AnimalResultsService();
            _climateService = new ClimateService();
            _fieldComponentHelper = new FieldComponentHelper();
        }

        #endregion

        #region Properties

        public List<AnimalComponentEmissionsResults> AnimalResults { get; set; }

        public IAnimalService AnimalResultsService { get; set; }

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
                    this.CombineInputsForAllCropsInSameYear(farm, detailViewItems);

                    // Merge multiple items with the same year into a single year view items so that no two view items have the same year when calculating ICBM results (ICBM calculations
                    // require exactly one item per year (with combined inputs when there is a secondary crop grown))
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

        public double CalculateTillageFactor(CropViewItem viewItem, Farm farm)
        {
            var soilData = farm.GetPreferredSoilData(viewItem);

            var result = _tillageFactorCalculator.CalculateTillageFactor(
                province: soilData.Province,
                soilFunctionalCategory: farm.GeographicData.DefaultSoilData.SoilFunctionalCategory,
                tillageType: viewItem.TillageType,
                cropType: viewItem.CropType,
                perennialYear: viewItem.YearInPerennialStand);

            return Math.Round(result, CoreConstants.DefaultNumberOfDecimalPlaces);
        }

        public double CalculateManagementFactor(double climateParameter, double tillageFactor)
        {
            var result = _climateParameterCalculator.CalculateClimateManagementFactor(climateParameter, tillageFactor);

            return Math.Round(result, CoreConstants.DefaultNumberOfDecimalPlaces);
        }

        #endregion

        #region Private Methods

        #endregion
    }
}
