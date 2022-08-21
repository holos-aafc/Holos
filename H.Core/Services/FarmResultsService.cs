using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using AutoMapper;
using AutoMapper.Execution;
using H.Core.Calculators.Economics;
using H.Core.Calculators.UnitsOfMeasurement;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Events;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers;
using H.Core.Providers.Animals;
using H.Core.Providers.Climate;
using H.Core.Providers.Feed;
using H.Core.Providers.Soil;
using H.Core.Services.Animals;
using H.Core.Services.LandManagement;
using Prism.Events;

namespace H.Core.Services
{
    public class FarmResultsService : IFarmResultsService
    {
        #region Fields

        private readonly IFieldComponentHelper _fieldComponentHelper = new FieldComponentHelper();
        private readonly IAnimalComponentHelper _animalComponentHelper = new AnimalComponentHelper();

        private readonly IFieldResultsService _fieldResultsService;
        private readonly AnimalResultsService _animalResultsService = new AnimalResultsService();

        private readonly IDietProvider _dietProvider = new DietProvider();
        private readonly Table_9_ManureTypes_Default_Composition_Provider _defaultManureCompositionProvider = new Table_9_ManureTypes_Default_Composition_Provider();
        private readonly Table_33_Default_Bedding_Material_Composition_Provider _defaultBeddingMaterialCompositionProvider = new Table_33_Default_Bedding_Material_Composition_Provider();

        private readonly IMapper _farmMapper;
        private readonly IMapper _defaultsMapper;
        private readonly IMapper _detailsScreenCropViewItemMapper;
        private readonly IMapper _dailyClimateDataMapper;
        private readonly IMapper _soilDataMapper;
        private readonly IMapper _customYieldDataMapper;
        private readonly IMapper _climateDataMapper;
        private readonly IMapper _geographicDataMapper;

        private readonly Dictionary<Farm, FarmEmissionResults> _farmEmissionResultsCache = new Dictionary<Farm, FarmEmissionResults>();
        private readonly IEventAggregator _eventAggregator;

        private readonly EconomicsCalculator _economicsCalculator;
        private readonly UnitsOfMeasurementCalculator _unitsCalculator = new UnitsOfMeasurementCalculator();
        #endregion

        #region Constructors
        public FarmResultsService(IEventAggregator eventAggregator, IFieldResultsService fieldResultsService)
        {
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

            var farmMapperConfiguration = new MapperConfiguration(x =>
            {
                x.CreateMap<Farm, Farm>()
                 .ForMember(y => y.Name, z => z.Ignore())
                 .ForMember(y => y.Guid, z => z.Ignore())
                 .ForMember(y => y.Defaults, z => z.Ignore())
                 .ForMember(y => y.StageStates, z => z.Ignore())
                 .ForMember(y => y.ClimateData, z => z.Ignore())
                 .ForMember(y => y.GeographicData, z => z.Ignore())
                 .ForMember(y => y.Components, z => z.Ignore());

            });

            _farmMapper = farmMapperConfiguration.CreateMapper();

            var defaultMapperConfiguration = new MapperConfiguration(x => { x.CreateMap<Defaults, Defaults>(); });

            _defaultsMapper = defaultMapperConfiguration.CreateMapper();

            var detailsScreenCropViewItemMapperConfiguration = new MapperConfiguration(x =>
                {
                    x.CreateMap<CropViewItem, CropViewItem>();
                });

            _detailsScreenCropViewItemMapper = detailsScreenCropViewItemMapperConfiguration.CreateMapper();

            #region Climate Mappers

            var climateDataMapper = new MapperConfiguration(x =>
            {
                x.CreateMap<ClimateData, ClimateData>()
                            .ForMember(y => y.DailyClimateData, z => z.Ignore());
            });
            _climateDataMapper = climateDataMapper.CreateMapper();

            var dailyclimateDataMapper = new MapperConfiguration(x =>
            {
                x.CreateMap<DailyClimateData, DailyClimateData>();
            });
            _dailyClimateDataMapper = dailyclimateDataMapper.CreateMapper();

            #endregion

            #region GeographicData Mappers

            var geographicDataMapper = new MapperConfiguration(x =>
            {
                x.CreateMap<GeographicData, GeographicData>()
                            .ForMember(y => y.SoilDataForAllComponentsWithinPolygon, z => z.Ignore())
                            .ForMember(y => y.DefaultSoilData, z => z.Ignore())
                            .ForMember(y => y.CustomYieldData, z => z.Ignore());
            });
            _geographicDataMapper = geographicDataMapper.CreateMapper();

            var soilDataMapper = new MapperConfiguration(x =>
            {
                x.CreateMap<SoilData, SoilData>();
            });
            _soilDataMapper = soilDataMapper.CreateMapper();

            var customYieldMapper = new MapperConfiguration(x =>
            {
                x.CreateMap<CustomUserYieldData, CustomUserYieldData>();
            });
            _customYieldDataMapper = customYieldMapper.CreateMapper();

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

            if (farm.ResultsCalculated)
            {
                if (_farmEmissionResultsCache.ContainsKey(farm))
                {
                    Trace.TraceInformation($"{nameof(FarmResultsService)}.{nameof(CalculateFarmEmissionResults)}: results already calculated for farm '{farm.Name}'. Returning cached results");

                    // Return cached results is calculations have already been made and no properties on any of the farm's components have changed
                    return _farmEmissionResultsCache[farm];
                }

                // Farm has results calculated flag set to true but no cached results (because system just started). Recalculate results now
            }

            Trace.TraceInformation($"{nameof(FarmResultsService)}.{nameof(CalculateFarmEmissionResults)}: calculating results for farm: '{farm.Name}'");

            if (farm.Components.Any() == false)
            {
                Trace.TraceInformation($"{nameof(FarmResultsService)}.{nameof(CalculateFarmEmissionResults)}: no components for farm: '{farm.Name}' found.");
            }

            // Components
            farmResults.FieldComponentEmissionResults.AddRange(_fieldResultsService.CalculateResultsForFieldComponent(farm));
            farmResults.AnimalComponentEmissionsResults.AddRange(_animalResultsService.GetAnimalResults(farm));

            // N20 emissions
            farmResults.MineralN2OEmissionsResults = _fieldResultsService.CalculateMineralN2OEmissionsForFarm(farmResults);
            farmResults.ManureN2OEmissionResults = _fieldResultsService.CalculateManureN2OEmissionsForFarm(farmResults);

            // Field results
            var finalFieldResults = _fieldResultsService.CalculateFinalResults(farm);
            farmResults.FinalFieldResultViewItems.AddRange(finalFieldResults);

            // Manure calculations - must be calculated after both field and animal results have been calculated.
            this.UpdateStorageTanks(farmResults);

            // Energy
            farmResults.FarmEnergyResults.EnergyCarbonDioxideFromManureApplication = farmResults.AnimalComponentEmissionsResults.TotalCarbonDioxideEmissionsFromManureSpreading();
            farmResults.FarmEnergyResults.TotalOnFarmCroppingEnergyEmissionsForFarm = farmResults.FieldComponentEmissionResults.TotalOnFarmCroppingEnergyEmissions();

            // Economics
            farmResults.EconomicResultsViewItems.AddRange(_economicsCalculator.CalculateCropResults(_fieldResultsService, farm));
            farmResults.EconomicsProfit = _economicsCalculator.GetTotalProfit(farmResults.EconomicResultsViewItems);

            _farmEmissionResultsCache[farm] = farmResults;

            _eventAggregator.GetEvent<FarmResultsCalculatedEvent>().Publish(new FarmResultsCalculatedEventArgs() { FarmEmissionResults = farmResults });

            Trace.TraceInformation($"{nameof(FarmResultsService)}.{nameof(CalculateFarmEmissionResults)}: results for farm: '{farm.Name}' calculated. {farmResults.ToString()}");

            return farmResults;
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

        public Farm Create()
        {
            var farm = new Farm();
            farm.Initialize();

            farm.DateModified = DateTime.Now;
            farm.DateCreated = DateTime.Now;

            farm.Diets.AddRange(_dietProvider.GetDiets());
            farm.DefaultManureCompositionData.AddRange(_defaultManureCompositionProvider.ManureCompositionData);
            farm.DefaultsCompositionOfBeddingMaterials.AddRange(_defaultBeddingMaterialCompositionProvider.Data);

            return farm;
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
                var replicatedFieldSystemComponent = _fieldComponentHelper.Replicate(fieldSystemComponent);

                replicatedFarm.Components.Add(replicatedFieldSystemComponent);
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

        public void UpdateManureTanksFromUserDefinedManureApplications(FarmEmissionResults farmEmissionResults)
        {
            var farm = farmEmissionResults.Farm;

            // Go over each field on the farm and subtract any field application made with the manure
            foreach (var fieldComponent in farm.FieldSystemComponents)
            {
                foreach (var viewItem in fieldComponent.CropViewItems)
                {
                    foreach (var manureApplicationViewItem in viewItem.ManureApplicationViewItems)
                    {
                        // If manure was imported, don't update local manure storage tanks
                        if (manureApplicationViewItem.ManureLocationSourceType == ManureLocationSourceType.Imported || manureApplicationViewItem.AnimalType == AnimalType.NotSelected)
                        {
                            continue;
                        }

                        // Get the correct manure tank based on the animal type of manure that was applied
                        var tank = farmEmissionResults.GetManureTankByAnimalType(manureApplicationViewItem.AnimalType, viewItem.Year);

                        // Get the amount of nitrogen applied
                        var amountOfNitrogenAppliedPerHectare = manureApplicationViewItem.AmountOfNitrogenAppliedPerHectare;

                        var totalNitrogenApplied = amountOfNitrogenAppliedPerHectare * viewItem.Area;

                        // Subtract the amount that was applied
                        tank.NitrogenSumOfAllManureApplicationsMade += totalNitrogenApplied;
                    }
                }
            }
        }

        /// <summary>
        /// Updates manure storage tank accounting based on amount of manure applied during field application(s) and grazing animals.
        /// </summary>
        public void UpdateStorageTanks(FarmEmissionResults farmEmissionResults)
        {
            this.InitializeAllManureTanks(farmEmissionResults);            
            this.UpdateManureTanksFromUserDefinedManureApplications(farmEmissionResults);            
        }

        private void InitializeAllManureTanks(FarmEmissionResults farmEmissionResults)
        {
            var farm = farmEmissionResults.Farm;

            var animalComponentCategories = new List<ComponentCategory>()
            {
                ComponentCategory.BeefProduction,
                ComponentCategory.Dairy,
                ComponentCategory.Swine,
                ComponentCategory.Sheep,
                ComponentCategory.Poultry,
                ComponentCategory.OtherLivestock
            };

            /*
             * Set the state of the tanks as if there had been no field applications made
             */

            var years = new List<int>();
            foreach (var farmFieldSystemComponent in farm.FieldSystemComponents)
            {
                foreach (var viewItems in farmFieldSystemComponent.CropViewItems)
                {
                    years.Add(viewItems.Year);
                }
            }

            var distinctYears = years.Distinct();
            foreach (var distinctYear in distinctYears)
            {
                foreach (var componentCategory in animalComponentCategories)
                {
                    // Get the animal component results for a particular category of animal components
                    var animalComponentResultsByCategory = farmEmissionResults.AnimalComponentEmissionsResults.Where(x => x.Component.ComponentCategory == componentCategory).ToList();

                    // Get the manure tank associated with the animal category
                    var tank = farmEmissionResults.GetManureTankByAnimalType(componentCategory.GetAnimalTypeFromComponentCategory(), distinctYear);

                    // Set the state of the tank as if no manure applications had been made
                    this.SetStartingStateOfManureTank(
                        manureTank: tank,
                        animalComponentResults: animalComponentResultsByCategory);
                }
            }
        }

        /// <summary>
        /// Set the state of the tanks as if there had been no field applications made.
        /// </summary>
        public void SetStartingStateOfManureTank(
            ManureTank manureTank,
            IList<AnimalComponentEmissionsResults> animalComponentResults)
        {
            manureTank.NitrogenSumOfAllManureApplicationsMade = 0;

            var totalOrganicNitrogenAvailableForLandApplication = 0.0;
            var totalTanAvailableForLandApplication = 0.0;
            var totalAmountOfCarbonInStoredManure = 0.0;
            var totalAvailableManureNitrogenInStoredManureAvailableForLandApplication = 0.0;
            var yearOfTank = manureTank.Year;

            // Don't include manure that is on pasture in the total amounts available
            foreach (var componentResults in animalComponentResults)
            {
                foreach (var allGroupResults in componentResults.EmissionResultsForAllAnimalGroupsInComponent)
                {
                    foreach (var groupEmissionsByMonth in allGroupResults.GroupEmissionsByMonths.Where(x => x.MonthsAndDaysData.ManagementPeriod.HousingDetails.HousingType != HousingType.Pasture && x.MonthsAndDaysData.Year == yearOfTank))
                    {
                        totalOrganicNitrogenAvailableForLandApplication += groupEmissionsByMonth.MonthlyOrganicNitrogenAvailableForLandApplication;
                        totalTanAvailableForLandApplication += groupEmissionsByMonth.MonthlyTanAvailableForLandApplication;
                        totalAmountOfCarbonInStoredManure += groupEmissionsByMonth.TotalAmountOfCarbonInStoredManure;
                        totalAvailableManureNitrogenInStoredManureAvailableForLandApplication += groupEmissionsByMonth.MonthlyNitrogenAvailableForLandApplication;
                    }
                }
            }

            manureTank.TotalOrganicNitrogenAvailableForLandApplication = totalOrganicNitrogenAvailableForLandApplication;
            manureTank.TotalTanAvailableForLandApplication = totalTanAvailableForLandApplication;
            manureTank.TotalAmountOfCarbonInStoredManure = totalAmountOfCarbonInStoredManure;

            // Before considering any manure applications, these values are the same
            manureTank.TotalAvailableManureNitrogenAvailableForLandApplication = totalAvailableManureNitrogenInStoredManureAvailableForLandApplication;
            manureTank.TotalAvailableManureNitrogenAvailableForLandApplicationAfterAllLandApplications = totalAvailableManureNitrogenInStoredManureAvailableForLandApplication;
        }

        #endregion
    }
}