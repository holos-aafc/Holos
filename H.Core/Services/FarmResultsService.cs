using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Navigation;
using H.Core.Mappers;
using H.Core.Calculators.Economics;
using H.Core.Calculators.Infrastructure;
using H.Core.Calculators.Nitrogen;
using H.Core.Calculators.UnitsOfMeasurement;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Events;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.Infrastructure;
using H.Core.Models.LandManagement.Fields;
using H.Core.Models.Results;
using H.Core.Providers;
using H.Core.Providers.Animals;
using H.Core.Providers.Climate;
using H.Core.Providers.Evapotranspiration;
using H.Core.Providers.Feed;
using H.Core.Providers.Precipitation;
using H.Core.Providers.Soil;
using H.Core.Providers.Temperature;
using H.Core.Services.Animals;
using H.Core.Services.Initialization;
using H.Core.Services.LandManagement;
using H.Infrastructure;
using Prism.Events;

namespace H.Core.Services
{
    public class FarmResultsService : IFarmResultsService
    {
        #region Fields

        private readonly IManureService _manureService;

        private readonly IFieldComponentHelper _fieldComponentHelper = new FieldComponentHelper();
        private readonly IAnimalComponentHelper _animalComponentHelper = new AnimalComponentHelper();
        private readonly IAnaerobicDigestionComponentHelper _anaerobicDigestionComponentHelper = new AnaerobicDigestionComponentHelper();

        private readonly IFieldResultsService _fieldResultsService;
        private readonly IAnimalService _animalResultsService;
        private readonly IADCalculator _adCalculator;

        private readonly ModelMapper<Farm> _farmMapper;
        private readonly ModelMapper<Defaults> _defaultsMapper;
        private readonly ModelMapper<CropViewItem> _detailsScreenCropViewItemMapper;
        private readonly ModelMapper<DailyClimateData> _dailyClimateDataMapper;
        private readonly ModelMapper<SoilData> _soilDataMapper;
        private readonly ModelMapper<CustomUserYieldData> _customYieldDataMapper;
        private readonly ModelMapper<ClimateData> _climateDataMapper;
        private readonly ModelMapper<GeographicData> _geographicDataMapper;

        private readonly IEventAggregator _eventAggregator;

        private readonly EconomicsCalculator _economicsCalculator;
        private IN2OEmissionFactorCalculator _n2OEmissionFactorCalculator;

        #endregion

        #region Constructors
        public FarmResultsService(IEventAggregator eventAggregator, IFieldResultsService fieldResultsService, IADCalculator adCalculator, IManureService manureService, IAnimalService animalService, IN2OEmissionFactorCalculator n2OEmissionFactorCalculator)
        {
            if (n2OEmissionFactorCalculator != null)
            {
                _n2OEmissionFactorCalculator = n2OEmissionFactorCalculator; 
            }
            else
            {
                throw new ArgumentNullException(nameof(n2OEmissionFactorCalculator));
            }

            if (animalService != null)
            {
                _animalResultsService = animalService;
            }
            else
            {
                throw new ArgumentNullException(nameof(animalService));
            }

            if (manureService != null)
            {
                _manureService = manureService;
            }
            else
            {
                throw new ArgumentNullException(nameof(manureService));
            }

            if (adCalculator != null)
            {
                _adCalculator = adCalculator;
            }
            else
            {
                throw new ArgumentNullException(nameof(adCalculator));
            }

            if (fieldResultsService != null)
            {
                _fieldResultsService = fieldResultsService;
                _economicsCalculator = new EconomicsCalculator(_fieldResultsService);
            }
            else
            {
                throw new ArgumentNullException(nameof(fieldResultsService));
            }

            if (eventAggregator != null)
            {
                _eventAggregator = eventAggregator;
            }
            else
            {
                throw new ArgumentNullException(nameof(eventAggregator));
            }

            #region Mappers

            // Farm's Diets / DefaultManureCompositionData / DefaultsCompositionOfBeddingMaterials were deep-copied by the
            // old bundled config (it registered Diet/DefaultManureCompositionData/Table_30 alongside Farm). ReplicateFarm
            // now reproduces those deep copies with explicit clone loops, so this mapper only needs the Farm map itself.
            _farmMapper = new ModelMapper<Farm>(
                nameof(Farm.Name), nameof(Farm.Guid), nameof(Farm.Defaults), nameof(Farm.StageStates),
                nameof(Farm.ClimateData), nameof(Farm.GeographicData), nameof(Farm.Components));

            _defaultsMapper = new ModelMapper<Defaults>();

            _detailsScreenCropViewItemMapper = new ModelMapper<CropViewItem>();

            // ClimateData's Precipitation/Temperature/Evapotranspiration/BarnTemperature were deep-copied by the old
            // bundled config; ReplicateFarm reproduces those with explicit clones after this map runs.
            _climateDataMapper = new ModelMapper<ClimateData>(
                nameof(ClimateData.DailyClimateData), nameof(ClimateData.Guid));

            _dailyClimateDataMapper = new ModelMapper<DailyClimateData>();

            _geographicDataMapper = new ModelMapper<GeographicData>(
                nameof(GeographicData.SoilDataForAllComponentsWithinPolygon),
                nameof(GeographicData.DefaultSoilData),
                nameof(GeographicData.CustomYieldData),
                nameof(GeographicData.Guid));

            _soilDataMapper = new ModelMapper<SoilData>();

            _customYieldDataMapper = new ModelMapper<CustomUserYieldData>();

            #endregion
        }

        #endregion

        #region Properties

        public bool CropEconomicDataApplied { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Calculates final results for a farm.
        /// </summary>
        public FarmEmissionResults CalculateFarmEmissionResults(Farm farm)
        {
            var farmResults = new FarmEmissionResults();
            farmResults.Farm = farm;

            if (farm.PolygonId == 0)
            {
                // Error
                return farmResults;
            }

            Trace.TraceInformation($"{nameof(FarmResultsService)}.{nameof(CalculateFarmEmissionResults)}: calculating results for farm: '{farm.Name}'");

            if (farm.Components.Any() == false)
            {
                Trace.TraceInformation($"{nameof(FarmResultsService)}.{nameof(CalculateFarmEmissionResults)}: no components for farm: '{farm.Name}' found.");
            }

            // Field results will use animal results to calculate indirect emissions from land applied manure. We will need to reset the animal component calculation state here.
            farm.ResetAnimalResults();

            var animalResults = _animalResultsService.GetAnimalResults(farm);

            farmResults.AnimalComponentEmissionsResults.AddRange(animalResults);
            _fieldResultsService.AnimalResults = animalResults;

            //var a = farmResults.GetDailyPrint();

            farmResults.AnaerobicDigestorResults.AddRange(this.CalculateAdResults(farm, animalResults.ToList()));

            farmResults.FinalFieldResultViewItems.AddRange(this.CalculateFieldResults(farm));

            // Manure calculations - must be calculated after both field and animal results have been calculated.
            _manureService.Initialize(farm, animalResults);

            farmResults.ManureExportResultsViewItems.AddRange(this.CalculateManureExportEmissions(farm));

            // Economics
            farmResults.EconomicResultsViewItems.AddRange(_economicsCalculator.CalculateCropResults(farmResults));
            farmResults.EconomicsProfit = _economicsCalculator.GetTotalProfit(farmResults.EconomicResultsViewItems);

            _eventAggregator.GetEvent<FarmResultsCalculatedEvent>().Publish(new FarmResultsCalculatedEventArgs() { FarmEmissionResults = farmResults });

            Trace.TraceInformation($"{nameof(FarmResultsService)}.{nameof(CalculateFarmEmissionResults)}: results for farm: '{farm.Name}' calculated. {farmResults.ToString()}");

            return farmResults;
        }

        public List<ManureExportResultViewItem> CalculateManureExportEmissions(Farm farm)
        {
            var result = new List<ManureExportResultViewItem>();

            foreach (var manureExportViewItem in farm.ManureExportViewItems)
            {
                var manureExportResultItem = new ManureExportResultViewItem
                {
                    DateOfExport = manureExportViewItem.DateOfExport,
                };

                manureExportResultItem.DirectN2ON = _n2OEmissionFactorCalculator.CalculateTotalDirectN2ONFromExportedManure(farm, manureExportViewItem);
                manureExportResultItem.IndirectN2ON = _n2OEmissionFactorCalculator.CalculateTotalIndirectN2ONFromExportedManure(farm, manureExportViewItem);
                manureExportResultItem.NitrateLeachedEmissions = _n2OEmissionFactorCalculator.CalculateTotalNitrateLeachedFromExportedManure(farm, manureExportViewItem);
                manureExportResultItem.VolatilizationEmissions = _n2OEmissionFactorCalculator.CalculateVolatilizationEmissionsFromExportedManure(farm, manureExportViewItem);
                manureExportResultItem.AdjustedVolatilizationEmissions = _n2OEmissionFactorCalculator.CalculateAdjustedNH3NLossFromManureExports(farm, manureExportViewItem);

                result.Add(manureExportResultItem);
            }

            return result;
        }

        public List<CropResidueExportResultViewItem> CalculateCropResidueExportViewItems(Farm farm)
        {
            var result = new List<CropResidueExportResultViewItem>();

            return result;
        }

        public List<CropViewItem> CalculateFieldResults(Farm farm)
        {
            // Field results
            var finalFieldResults = _fieldResultsService.CalculateFinalResults(farm);

            return finalFieldResults;
        }

        public List<DigestorDailyOutput> CalculateAdResults(Farm farm, List<AnimalComponentEmissionsResults> animalComponentEmissionsResults)
        {
            return _adCalculator.CalculateResults(farm, animalComponentEmissionsResults);
        }

        /// <summary>
        /// Calculates final results for a collection of farms.
        /// </summary>
        public List<FarmEmissionResults> CalculateFarmEmissionResults(IEnumerable<Farm> farms)
        {
            var result = new List<FarmEmissionResults>();

            foreach (var farm in farms)
            {
                result.Add(this.CalculateFarmEmissionResults(farm));
            }

            return result;
        }

        public List<Farm> ReplicateFarms(IEnumerable<Farm> farms)
        {
            var result = new List<Farm>();

            foreach (var farm in farms)
            {
                result.Add(this.ReplicateFarm(farm));
            }

            return result;
        }

        public Farm ReplicateFarm(Farm farm)
        {
            var replicatedFarm = new Farm
            {
                GeographicData = new GeographicData()
            };

            _farmMapper.Map(farm, replicatedFarm);
            _defaultsMapper.Map(farm.Defaults, replicatedFarm.Defaults);
            _climateDataMapper.Map(farm.ClimateData, replicatedFarm.ClimateData);
            _geographicDataMapper.Map(farm.GeographicData, replicatedFarm.GeographicData);

            replicatedFarm.Name = farm.Name;

            // Reproduce the deep copies the previous bundled AutoMapper configurations performed. The Farm and
            // ClimateData maps above are shallow, so without these explicit clones the replicated farm would share these
            // references (and their list elements) with the original farm.
            replicatedFarm.ClimateData.PrecipitationData = PropertyMapper.Clone(farm.ClimateData.PrecipitationData);
            replicatedFarm.ClimateData.TemperatureData = PropertyMapper.Clone(farm.ClimateData.TemperatureData);
            replicatedFarm.ClimateData.EvapotranspirationData = PropertyMapper.Clone(farm.ClimateData.EvapotranspirationData);
            replicatedFarm.ClimateData.BarnTemperatureData = PropertyMapper.Clone(farm.ClimateData.BarnTemperatureData);

            replicatedFarm.Diets.Clear();
            foreach (var diet in farm.Diets)
            {
                replicatedFarm.Diets.Add(PropertyMapper.Clone(diet));
            }

            replicatedFarm.DefaultManureCompositionData.Clear();
            foreach (var defaultManureCompositionData in farm.DefaultManureCompositionData)
            {
                replicatedFarm.DefaultManureCompositionData.Add(PropertyMapper.Clone(defaultManureCompositionData));
            }

            replicatedFarm.DefaultsCompositionOfBeddingMaterials.Clear();
            foreach (var beddingMaterialComposition in farm.DefaultsCompositionOfBeddingMaterials)
            {
                replicatedFarm.DefaultsCompositionOfBeddingMaterials.Add(PropertyMapper.Clone(beddingMaterialComposition));
            }

            #region Animal Components

            foreach (var animalComponent in farm.AnimalComponents.Cast<AnimalComponentBase>())
            {
                var replicatedAnimalComponent = _animalComponentHelper.ReplicateAnimalComponent(animalComponent);

                replicatedFarm.Components.Add(replicatedAnimalComponent);
            }

            #endregion

            #region FieldSystemComponents

            foreach (var fieldSystemComponent in farm.FieldSystemComponents)
            {
                // Need to restore the field GUID for grazing periods
                var replicatedFieldSystemComponent = _fieldComponentHelper.Replicate(fieldSystemComponent);

                var originalFieldGuid = fieldSystemComponent.Guid;
                var replicatedFieldGuid = replicatedFieldSystemComponent.Guid;
                foreach (var managementPeriod in farm.GetAllManagementPeriods().Where(x => x.HousingDetails.PastureLocation != null && x.HousingDetails.PastureLocation.Guid.Equals(originalFieldGuid)))
                {
                    managementPeriod.HousingDetails.PastureLocation.Guid = replicatedFieldGuid;
                }

                replicatedFarm.Components.Add(replicatedFieldSystemComponent);
            }

            #endregion

            #region AnaerobicDigestionComponents

            foreach (var anaerobicDigestionComponent in farm.AnaerobicDigestionComponents)
            {
                var replicatedAnaerobicDigestionComponent = _anaerobicDigestionComponentHelper.Replicate(anaerobicDigestionComponent, replicatedFarm.AnimalComponents);

                replicatedFarm.Components.Add(replicatedAnaerobicDigestionComponent);
            }

            #endregion

            #region StageStates

            foreach (var fieldSystemDetailsStageState in farm.StageStates.OfType<FieldSystemDetailsStageState>().ToList())
            {
                var stageState = new FieldSystemDetailsStageState();
                replicatedFarm.StageStates.Add(stageState);

                foreach (var detailsScreenViewCropViewItem in fieldSystemDetailsStageState.DetailsScreenViewCropViewItems)
                {
                    var viewItem = new CropViewItem();

                    _detailsScreenCropViewItemMapper.Map(detailsScreenViewCropViewItem, viewItem);

                    stageState.DetailsScreenViewCropViewItems.Add(viewItem);
                }
            }

            #endregion

            #region GeographicData

            foreach (var soilData in farm.GeographicData.SoilDataForAllComponentsWithinPolygon)
            {
                var replicatedSoilData = new SoilData();
                _soilDataMapper.Map(soilData, replicatedSoilData);
                replicatedFarm.GeographicData.SoilDataForAllComponentsWithinPolygon.Add(replicatedSoilData);
            }

            _soilDataMapper.Map(farm.GeographicData.DefaultSoilData, replicatedFarm.GeographicData.DefaultSoilData);

            foreach (var customYieldData in farm.GeographicData.CustomYieldData)
            {
                var replicatedCustomYieldData = new CustomUserYieldData();
                _customYieldDataMapper.Map(customYieldData, replicatedCustomYieldData);
                replicatedFarm.GeographicData.CustomYieldData.Add(replicatedCustomYieldData);
            }

            #endregion

            #region ClimateData

            foreach (var dailyClimateData in farm.ClimateData.DailyClimateData)
            {
                var replicatedDailyClimateData = new DailyClimateData();
                _dailyClimateDataMapper.Map(dailyClimateData, replicatedDailyClimateData);
                replicatedFarm.ClimateData.DailyClimateData.Add(dailyClimateData);
            }

            #endregion

            return replicatedFarm;
        }

        #endregion
    }
}