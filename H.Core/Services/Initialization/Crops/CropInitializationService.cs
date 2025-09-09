using System.Diagnostics;
using AutoMapper;
using H.Core.Calculators.Carbon;
using H.Core.Calculators.Economics;
using H.Core.Calculators.Nitrogen;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Carbon;
using H.Core.Providers.Economics;
using H.Core.Providers.Energy;
using H.Core.Providers.Fertilizer;
using H.Core.Providers.Nitrogen;
using H.Core.Providers.Plants;
using H.Core.Providers.Soil;
using H.Core.Services.Animals;
using H.Core.Services.LandManagement;
using Microsoft.Extensions.Logging.Abstractions;

namespace H.Core.Services.Initialization.Crops
{
    public partial class CropInitializationService : ICropInitializationService
    {
        #region Private Methods

        private Table_7_Relative_Biomass_Information_Data GetResidueData(Farm farm, CropViewItem viewItem)
        {
            var soilFunctionCategory = farm.GetPreferredSoilData(viewItem).SoilFunctionalCategory;
            var totalWater = _irrigationService.GetTotalWaterInputs(farm, viewItem);
            var residueData = _relativeBiomassInformationProvider.GetResidueData(viewItem.IrrigationType, totalWater,
                viewItem.CropType, soilFunctionCategory, farm.Province);

            return residueData;
        }

        #endregion

        #region Fields

        private readonly LandManagementChangeHelper _landManagementChangeHelper;

        private readonly LumCMax_KValues_Perennial_Cropping_Change_Provider
            _lumCMaxKValuesPerennialCroppingChangeProvider;

        private readonly LumCMax_KValues_Fallow_Practice_Change_Provider _lumCMaxKValuesFallowPracticeChangeProvider;
        private readonly EcodistrictDefaultsProvider _ecodistrictDefaultsProvider;
        private readonly Table_7_Relative_Biomass_Information_Provider _relativeBiomassInformationProvider;
        private readonly ICBMCarbonInputCalculator _icbmCarbonInputCalculator;
        private readonly EconomicsHelper _economicsHelper;
        private readonly IICBMNitrogenInputCalculator _icbmNitrogenInputCalculator;
        private readonly NitogenFixationProvider _nitrogenFixationProvider;
        private readonly Table_9_Nitrogen_Lignin_Content_In_Crops_Provider _slopeProviderTable;
        private readonly IrrigationService _irrigationService;

        private readonly Table_60_Utilization_Rates_For_Livestock_Grazing_Provider
            _utilizationRatesForLivestockGrazingProvider;

        private readonly IManureService _manureService;

        private static readonly SmallAreaYieldProvider _smallAreaYieldProvider;
        private static readonly CropEconomicsProvider _economicsProvider;
        private static readonly Table_51_Herbicide_Energy_Estimates_Provider _herbicideEnergyEstimatesProvider;

        private static readonly Table_48_Carbon_Footprint_For_Fertilizer_Blends_Provider
            _carbonFootprintForFertilizerBlendsProvider;

        private static readonly Table_50_Fuel_Energy_Estimates_Provider _fuelEnergyEstimatesProvider;

        private readonly IMapper _soilDataMapper;

        #endregion

        #region Constructors

        static CropInitializationService()
        {
            _smallAreaYieldProvider = new SmallAreaYieldProvider();
            _smallAreaYieldProvider.Initialize();

            _economicsProvider = new CropEconomicsProvider();
            _herbicideEnergyEstimatesProvider = new Table_51_Herbicide_Energy_Estimates_Provider();
            _carbonFootprintForFertilizerBlendsProvider =
                new Table_48_Carbon_Footprint_For_Fertilizer_Blends_Provider();
            _fuelEnergyEstimatesProvider = new Table_50_Fuel_Energy_Estimates_Provider();
        }

        public CropInitializationService()
        {
            _relativeBiomassInformationProvider = new Table_7_Relative_Biomass_Information_Provider();
            _landManagementChangeHelper = new LandManagementChangeHelper();
            _lumCMaxKValuesFallowPracticeChangeProvider = new LumCMax_KValues_Fallow_Practice_Change_Provider();
            _lumCMaxKValuesPerennialCroppingChangeProvider = new LumCMax_KValues_Perennial_Cropping_Change_Provider();
            _ecodistrictDefaultsProvider = new EcodistrictDefaultsProvider();
            _icbmCarbonInputCalculator = new ICBMCarbonInputCalculator();
            _economicsHelper = new EconomicsHelper();
            _icbmNitrogenInputCalculator = new ICBMNitrogenInputCalculator();
            _nitrogenFixationProvider = new NitogenFixationProvider();
            _slopeProviderTable = new Table_9_Nitrogen_Lignin_Content_In_Crops_Provider();
            _irrigationService = new IrrigationService();
            _utilizationRatesForLivestockGrazingProvider =
                new Table_60_Utilization_Rates_For_Livestock_Grazing_Provider();
            _manureService = new ManureService();

            var soilDataMapper =
                new MapperConfiguration(x => { x.CreateMap<SoilData, SoilData>(); }, new NullLoggerFactory());

            _soilDataMapper = soilDataMapper.CreateMapper();
        }

        #endregion

        #region Public Methods

        public void InitializeCrops(Farm farm, GlobalSettings globalSettings)
        {
            if (farm != null)
            {
                foreach (var viewItem in farm.GetAllCropViewItems()) InitializeCrop(viewItem, farm, globalSettings);

                // Initialize field level properties (soil, etc.) not found at the crop level
                foreach (var fieldSystemComponent in farm.FieldSystemComponents)
                    InitializeField(farm, fieldSystemComponent);
            }
        }

        public void InitializeField(Farm farm, FieldSystemComponent fieldSystemComponent)
        {
            InitializeDefaultSoilForField(farm, fieldSystemComponent);
        }

        /// <summary>
        ///     Applies the default properties on a crop view item based on system defaults and user defaults (if available). Any
        ///     property that cannot be set in the constructor
        ///     of the <see cref="H.Core.Models.LandManagement.Fields.CropViewItem" /> should be set here.
        /// </summary>
        public void InitializeCrop(CropViewItem viewItem, Farm farm, GlobalSettings globalSettings)
        {
            viewItem.IsInitialized = false;

            Trace.TraceInformation(
                $"{nameof(CropInitializationService)}.{nameof(InitializeCrop)}: applying defaults to {viewItem.CropTypeString}");

            InitializeNitrogenFixation(viewItem);
            InitializeCarbonConcentration(viewItem, farm.Defaults);
            InitializeIrrigationWaterApplication(farm, viewItem);
            InitializeBiomassCoefficients(viewItem, farm);
            InitializeNitrogenContent(viewItem, farm);
            InitializeSoilProperties(viewItem, farm);
            InitializePercentageReturns(farm, viewItem);
            InitializeMoistureContent(viewItem, farm);
            InitializeTillageType(viewItem, farm);
            InitializeYield(viewItem, farm);
            InitializeHerbicideEnergy(farm, viewItem);
            InitializeFuelEnergy(farm, viewItem);
            InitializeFallow(viewItem, farm);
            InitializePerennialDefaults(viewItem, farm);
            InitializeHarvestMethod(viewItem);
            InitializeLigninContent(viewItem, farm);
            InitializeEconomicDefaults(viewItem, farm);
            InitializeUserDefaults(viewItem, globalSettings);

            viewItem.IsInitialized = true;
        }

        #endregion
    }
}