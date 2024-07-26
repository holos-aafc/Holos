using H.Core.Models.LandManagement.Fields;
using H.Core.Models;
using H.Core.Providers.Carbon;
using H.Core.Providers.Plants;
using H.Core.Providers.Soil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Calculators.Carbon;
using H.Core.Enumerations;
using H.Core.Calculators.Economics;
using H.Core.Providers.Economics;
using H.Core.Calculators.Nitrogen;
using H.Core.Providers.Fertilizer;
using H.Core.Providers.Nitrogen;
using H.Core.Providers.Energy;
using H.Core.Services.LandManagement;
using AutoMapper;

namespace H.Core.Services.Initialization.Crops
{
    public partial class CropInitializationService : ICropInitializationService
    {
        #region Fields

        private readonly LandManagementChangeHelper _landManagementChangeHelper;
        private readonly LumCMax_KValues_Perennial_Cropping_Change_Provider _lumCMaxKValuesPerennialCroppingChangeProvider;
        private readonly LumCMax_KValues_Fallow_Practice_Change_Provider _lumCMaxKValuesFallowPracticeChangeProvider;
        private readonly EcodistrictDefaultsProvider _ecodistrictDefaultsProvider;
        private readonly Table_7_Relative_Biomass_Information_Provider _relativeBiomassInformationProvider;
        private readonly SmallAreaYieldProvider _smallAreaYieldProvider;
        private readonly ICBMCarbonInputCalculator _icbmCarbonInputCalculator;
        private readonly EconomicsHelper _economicsHelper;
        private readonly CropEconomicsProvider _economicsProvider;
        private readonly Table_48_Carbon_Footprint_For_Fertilizer_Blends_Provider _carbonFootprintForFertilizerBlendsProvider;
        private readonly IICBMNitrogenInputCalculator _icbmNitrogenInputCalculator;
        private readonly NitogenFixationProvider _nitrogenFixationProvider;
        private readonly Table_9_Nitrogen_Lignin_Content_In_Crops_Provider _slopeProviderTable;
        private readonly Table_51_Herbicide_Energy_Estimates_Provider _herbicideEnergyEstimatesProvider;
        private readonly IrrigationService _irrigationService;
        private readonly Table_50_Fuel_Energy_Estimates_Provider _fuelEnergyEstimatesProvider;
        private readonly Table_60_Utilization_Rates_For_Livestock_Grazing_Provider _utilizationRatesForLivestockGrazingProvider;

        #endregion

        #region Constructors

        public CropInitializationService()
        {
            _relativeBiomassInformationProvider = new Table_7_Relative_Biomass_Information_Provider();
            _landManagementChangeHelper = new LandManagementChangeHelper();
            _lumCMaxKValuesFallowPracticeChangeProvider = new LumCMax_KValues_Fallow_Practice_Change_Provider();
            _lumCMaxKValuesPerennialCroppingChangeProvider = new LumCMax_KValues_Perennial_Cropping_Change_Provider();
            _ecodistrictDefaultsProvider = new EcodistrictDefaultsProvider();
            _smallAreaYieldProvider = new SmallAreaYieldProvider();
            _icbmCarbonInputCalculator = new ICBMCarbonInputCalculator();
            _economicsHelper = new EconomicsHelper();
            _economicsProvider = new CropEconomicsProvider();
            _carbonFootprintForFertilizerBlendsProvider = new Table_48_Carbon_Footprint_For_Fertilizer_Blends_Provider();
            _icbmNitrogenInputCalculator = new ICBMNitrogenInputCalculator();
            _nitrogenFixationProvider = new NitogenFixationProvider();
            _slopeProviderTable = new Table_9_Nitrogen_Lignin_Content_In_Crops_Provider();
            _herbicideEnergyEstimatesProvider = new Table_51_Herbicide_Energy_Estimates_Provider();
            _irrigationService = new IrrigationService();
            _fuelEnergyEstimatesProvider = new Table_50_Fuel_Energy_Estimates_Provider();
            _utilizationRatesForLivestockGrazingProvider = new Table_60_Utilization_Rates_For_Livestock_Grazing_Provider();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Applies the default properties on a crop view item based on Holos defaults and user defaults (if available). Any property that cannot be set in the constructor
        /// of the <see cref="H.Core.Models.LandManagement.Fields.CropViewItem"/> should be set here.
        /// </summary>
        public void InitializeCropDefaults(CropViewItem viewItem, Farm farm, GlobalSettings globalSettings)
        {
            viewItem.IsInitialized = false;

            Trace.TraceInformation($"{nameof(CropInitializationService)}.{nameof(InitializeCropDefaults)}: applying defaults to {viewItem.CropTypeString}");

            var defaults = farm.Defaults;

            this.InitializeNitrogenFixation(viewItem);
            this.InitializeCarbonConcentration(viewItem, defaults);
            this.InitializeIrrigationWaterApplication(farm, viewItem);
            this.InitializeBiomassCoefficients(viewItem, farm);
            this.InitializeNitrogenContent(viewItem, farm);
            this.InitializeSoilProperties(viewItem, farm);
            this.InitializePercentageReturns(farm, viewItem);
            this.InitializeMoistureContent(viewItem, farm);
            this.InitializeTillageType(viewItem, farm);
            this.InitializeYield(viewItem, farm);
            this.InitializeHerbicideEnergy(farm, viewItem);
            this.InitializeFuelEnergy(farm, viewItem);
            this.InitializeFallow(viewItem, farm);
            this.InitializePerennialDefaults(viewItem, farm);
            this.InitializeHarvestMethod(viewItem, farm);
            this.InitializeLigninContent(viewItem, farm);
            this.InitializeEconomicDefaults(viewItem, farm);
            this.InitializeUserDefaults(viewItem, globalSettings);

            viewItem.IsInitialized = true;
            viewItem.CropEconomicData.IsInitialized = true;
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

        public void InitializeUserDefaults(CropViewItem viewItem, GlobalSettings globalSettings)
        {
            // Check if user has defaults defined for the type of crop
            var cropDefaults = globalSettings.CropDefaults.SingleOrDefault(x => x.CropType == viewItem.CropType);
            if (cropDefaults == null)
            {
                return;
            }

            if (cropDefaults.EnableCustomUserDefaultsForThisCrop == false)
            {
                // User did not specify defaults for this crop (or just wants to use system defaults) so return from here without modifying the view item further

                return;
            }

            var customCropDefaultsMapperConfiguration = new MapperConfiguration(configuration =>
            {
                // Don't copy the GUID, and do not overwrite the year, name, or area, on the crop
                configuration.CreateMap<CropViewItem, CropViewItem>()
                    .ForMember(x => x.Guid, options => options.Ignore())
                    .ForMember(x => x.Year, options => options.Ignore())
                    .ForMember(x => x.Name, options => options.Ignore())
                    .ForMember(x => x.Area, options => options.Ignore());
            });

            var mapper = customCropDefaultsMapperConfiguration.CreateMapper();

            mapper.Map(cropDefaults, viewItem);
        }

        #endregion
    }
}
