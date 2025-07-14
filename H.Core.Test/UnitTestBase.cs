using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.Animals.Beef;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models.Animals.Dairy;
using H.Core.Models.Infrastructure;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Animals;
using H.Core.Providers.Climate;
using H.Core.Services;
using H.Core.Services.Animals;
using Moq;
using H.Core.Calculators.Carbon;
using H.Core.Calculators.Nitrogen;
using H.Core.Providers;
using H.Core.Providers.Evapotranspiration;
using H.Core.Providers.Feed;
using H.Core.Providers.Fertilizer;
using H.Core.Providers.Precipitation;
using H.Core.Providers.Soil;
using H.Core.Providers.Temperature;
using H.Core.Services.Initialization;
using H.Core.Services.LandManagement;

namespace H.Core.Test
{
    public abstract class UnitTestBase
    {
        #region Fields

        protected Mock<IFarmResultsService> _mockFarmResultService;
        protected IFarmResultsService _mockFarmResultServiceObject;
        protected Mock<IManureService> _mockManureService;
        protected Mock<IDigestateService> _mockDigestateService;
        protected IDigestateService _mockDigestateServiceObject;
        protected IManureService _mockManureServiceObject;
        protected Mock<IClimateProvider> _mockClimateProvider;
        protected IClimateProvider _mockClimateProviderObject;
        protected Mock<IAnimalEmissionFactorsProvider> _mockEmissionDataProvider;
        protected IAnimalEmissionFactorsProvider _mockEmissionDataProviderObject;
        protected Mock<IAnimalAmmoniaEmissionFactorProvider> _mockAnimalAmmoniaEmissionFactorProvider;
        protected IAnimalAmmoniaEmissionFactorProvider _mockAnimalAmmoniaEmissionFactorProviderObject;
        protected ClimateProvider _climateProvider;
        protected ICBMSoilCarbonCalculator _iCbmSoilCarbonCalculator;
        protected N2OEmissionFactorCalculator _n2OEmissionFactorCalculator;
        protected IPCCTier2SoilCarbonCalculator _ipcc;
        protected IFieldResultsService _fieldResultsService;
        protected Mock<ISlcClimateProvider> _slcClimateProvider;
        protected Mock<IInitializationService> _mockInitializationService;
        protected IInitializationService _initializationService;
        protected ICBMCarbonInputCalculator icbmCarbonInputCalculator;
        protected ICarbonService carbonService;

        #endregion

        #region Constructors

        static UnitTestBase()
        {
        }

        protected UnitTestBase()
        {
            icbmCarbonInputCalculator = new ICBMCarbonInputCalculator();
            _initializationService = new InitializationService();

            _mockFarmResultService = new Mock<IFarmResultsService>();
            _mockFarmResultServiceObject = _mockFarmResultService.Object;

            _mockManureService = new Mock<IManureService>();
            _mockManureServiceObject = _mockManureService.Object;

            _mockDigestateService = new Mock<IDigestateService>();
            _mockDigestateServiceObject = _mockDigestateService.Object;
            _mockDigestateService.Setup(x => x.GetValidDigestateLocationSourceTypes())
                .Returns(new List<ManureLocationSourceType>());

            _mockClimateProvider = new Mock<IClimateProvider>();
            _mockClimateProviderObject = _mockClimateProvider.Object;

            _mockEmissionDataProvider = new Mock<IAnimalEmissionFactorsProvider>();
            _mockEmissionDataProviderObject = _mockEmissionDataProvider.Object;

            _mockAnimalAmmoniaEmissionFactorProvider = new Mock<IAnimalAmmoniaEmissionFactorProvider>();
            _mockAnimalAmmoniaEmissionFactorProviderObject = _mockAnimalAmmoniaEmissionFactorProvider.Object;

            _slcClimateProvider = new Mock<ISlcClimateProvider>();
            _climateProvider = new ClimateProvider(_slcClimateProvider.Object);
            _n2OEmissionFactorCalculator = new N2OEmissionFactorCalculator(_climateProvider);
            _iCbmSoilCarbonCalculator = new ICBMSoilCarbonCalculator(_climateProvider, _n2OEmissionFactorCalculator);
            _ipcc = new IPCCTier2SoilCarbonCalculator(_climateProvider, _n2OEmissionFactorCalculator);

            _fieldResultsService = new Mock<IFieldResultsService>().Object;
            carbonService = new CarbonService();
        }

        #endregion

        #region Public Methods
        public Storage InitializeStorage()
        {
            var storage = new Storage
            {
                ApplicationData = new ApplicationData
                {
                    GlobalSettings = new GlobalSettings
                    {
                        ActiveFarm = new Farm()
                    }
                }
            };

            return storage;
        }

        public ManagementPeriod GetTestManagementPeriod()
        {
            var managementPeriod = new ManagementPeriod();

            managementPeriod.AnimalType = AnimalType.Beef;
            managementPeriod.Start = new DateTime(DateTime.Now.Year, 1, 1);
            managementPeriod.StartWeight = 100;
            managementPeriod.EndWeight = 200;
            managementPeriod.Duration = TimeSpan.FromDays(30);
            managementPeriod.HousingDetails = new HousingDetails();
            managementPeriod.HousingDetails.HousingType = HousingType.Pasture;
            managementPeriod.SelectedDiet = new Diet();
            managementPeriod.SelectedDiet.TotalDigestibleNutrient = 75;

            return managementPeriod;
        }

        public DigestateApplicationViewItem GetTestRawDigestateApplicationViewItem()
        {
            var digestateApplication = new DigestateApplicationViewItem();

            digestateApplication.DateCreated = DateTime.Now.AddDays(1);
            digestateApplication.DigestateState = DigestateState.Raw;
            digestateApplication.ManureLocationSourceType = ManureLocationSourceType.Livestock;
            digestateApplication.AmountAppliedPerHectare = 50;
            digestateApplication.AmountOfNitrogenAppliedPerHectare = 50;

            return digestateApplication;
        }

        public DigestateApplicationViewItem GetTestLiquidDigestateApplicationViewItem()
        {
            var digestateApplication = new DigestateApplicationViewItem();

            digestateApplication.DateCreated = DateTime.Now.AddDays(1);
            digestateApplication.DigestateState = DigestateState.LiquidPhase;
            digestateApplication.AmountAppliedPerHectare = 50;
            digestateApplication.AmountOfNitrogenAppliedPerHectare = 500;
            digestateApplication.ManureLocationSourceType = ManureLocationSourceType.Livestock;

            return digestateApplication;
        }

        public AnaerobicDigestionComponent GetTestAnaerobicDigestionComponent()
        {
            var component = new AnaerobicDigestionComponent();

            return component;
        }

        public Farm GetTestFarm()
        {
            var farm = new Farm();
            var backgroundingComponent = new BackgroundingComponent();
            var group = new AnimalGroup();
            @group.GroupType = AnimalType.BeefBackgrounderHeifer;

            var dairyComponent = new DairyComponent();
            var cowsGroup = new AnimalGroup();
            cowsGroup.GroupType = AnimalType.DairyLactatingCow;

            var managementPeriod = new ManagementPeriod();
            managementPeriod.Start = new DateTime(DateTime.Now.Year, 1, 1);
            managementPeriod.AnimalType = AnimalType.BeefBackgrounderHeifer;
            managementPeriod.StartWeight = 100;
            managementPeriod.NumberOfAnimals = 1000;
            managementPeriod.EndWeight = 200;
            managementPeriod.GainCoefficient = 0.4;
            managementPeriod.SelectedDiet = new Diet();
            managementPeriod.SelectedDiet.TotalDigestibleNutrient = 100;
            managementPeriod.SelectedDiet.CrudeProtein = 50;

            var timespan = TimeSpan.FromDays(30 * 2);
            managementPeriod.Duration = timespan;
            managementPeriod.End = managementPeriod.Start.Add(timespan);

            managementPeriod.ManureDetails.StateType = ManureStateType.DeepBedding;
            managementPeriod.ManureDetails.FractionOfNitrogenInManure = 0.5;

            farm.Components.Add(backgroundingComponent);
            farm.Components.Add(dairyComponent);

            backgroundingComponent.Groups.Add(group);
            dairyComponent.Groups.Add(cowsGroup);

            group.ManagementPeriods.Add(managementPeriod);

            /*
             * Manure exports
             */

            farm.ManureExportViewItems.Add(new ManureExportViewItem() { DateOfExport = DateTime.Now, Amount = 1000, AnimalType = AnimalType.Dairy, DefaultManureCompositionData = new DefaultManureCompositionData(){NitrogenContent = 0.5}});
            farm.ManureExportViewItems.Add(new ManureExportViewItem() { DateOfExport = DateTime.Now, Amount = 2000, AnimalType = AnimalType.Dairy, DefaultManureCompositionData = new DefaultManureCompositionData() { NitrogenContent = 0.5 } });

            return farm;
        }

        public FieldSystemDetailsStageState GetFieldStageState()
        {
            var stageState = new FieldSystemDetailsStageState();

            stageState.DetailsScreenViewCropViewItems = new ObservableCollection<CropViewItem>()
            {
                new CropViewItem()
                {
                    Year = 2023,
                }
            };

            return stageState;
        }

        public AnimalComponentBase GetTestGrazingAnimalComponent(FieldSystemComponent fieldSystemComponent)
        {
            var component = new BackgroundingComponent();

            var group = new AnimalGroup();

            component.Groups.Add(group);

            var managementPeriod = new ManagementPeriod();
            group.ManagementPeriods.Add(managementPeriod);

            managementPeriod.HousingDetails = new HousingDetails();
            managementPeriod.HousingDetails.HousingType = HousingType.Pasture;
            managementPeriod.HousingDetails.PastureLocation = fieldSystemComponent;

            return component;
        }

        public AnimalComponentEmissionsResults GetEmptyTestAnimalComponentEmissionsResults()
        {
            var results = new AnimalComponentEmissionsResults();

            var monthsAndDaysData = new MonthsAndDaysData();

            var managementPeriod = new ManagementPeriod();
            managementPeriod.HousingDetails = new HousingDetails();
            monthsAndDaysData.ManagementPeriod = managementPeriod;

            var groupEmissionsByDay = new GroupEmissionsByDay();

            var groupEmissionsByMonth = new GroupEmissionsByMonth(monthsAndDaysData, new List<GroupEmissionsByDay>() { groupEmissionsByDay });

            var animalGroupResults = new AnimalGroupEmissionResults();
            animalGroupResults.GroupEmissionsByMonths = new List<GroupEmissionsByMonth>() { groupEmissionsByMonth };

            results.EmissionResultsForAllAnimalGroupsInComponent = new List<AnimalGroupEmissionResults>() { animalGroupResults };

            return results;
        }

        public GroupEmissionsByDay GetGroupEmissionsByDay()
        {
            return  new GroupEmissionsByDay()
            {
                AdjustedAmountOfTanInStoredManureOnDay = 100,
                OrganicNitrogenCreatedOnDay = 50,
                TotalVolumeOfManureAvailableForLandApplication = 100,
            };
        }

        public AnimalComponentEmissionsResults GetNonEmptyTestBeefCattleAnimalComponentEmissionsResults()
        {
            var results = new AnimalComponentEmissionsResults();
            results.Component = new BackgroundingComponent();

            var monthsAndDaysData = new MonthsAndDaysData();
            monthsAndDaysData.Year = DateTime.Now.Year;

            var managementPeriod = new ManagementPeriod();
            managementPeriod.HousingDetails = new HousingDetails();
            managementPeriod.HousingDetails.HousingType = HousingType.Confined;
            monthsAndDaysData.ManagementPeriod = managementPeriod;

            managementPeriod.ManureDetails.StateType = ManureStateType.AnaerobicDigester;

            var groupEmissionsByMonth = new GroupEmissionsByMonth(monthsAndDaysData, new List<GroupEmissionsByDay>() { this.GetGroupEmissionsByDay() });

            var animalGroupResults = new AnimalGroupEmissionResults();
            animalGroupResults.GroupEmissionsByMonths = new List<GroupEmissionsByMonth>() { groupEmissionsByMonth };

            results.EmissionResultsForAllAnimalGroupsInComponent = new List<AnimalGroupEmissionResults>() { animalGroupResults };

            return results;
        }

        public AnimalComponentEmissionsResults GetTestGrazingBeefCattleAnimalComponentEmissionsResults(FieldSystemComponent fieldSystemComponent)
        {
            var results = new AnimalComponentEmissionsResults();
            results.Component = this.GetTestGrazingAnimalComponent(fieldSystemComponent);

            var animalGroupResults = this.GetTestGrazingBeefCattleAnimalGroupComponentEmissionsResults(fieldSystemComponent);

            results.EmissionResultsForAllAnimalGroupsInComponent = new List<AnimalGroupEmissionResults>() { animalGroupResults };

            return results;
        }

        public AnimalGroupEmissionResults GetTestGrazingBeefCattleAnimalGroupComponentEmissionsResults(FieldSystemComponent fieldSystemComponent)
        {
            var animalGroupResults = new AnimalGroupEmissionResults();

            var groupEmissionsByMonth = this.GetTestGroupEmissionsByMonthForGrazingAnimals(fieldSystemComponent);
            animalGroupResults.GroupEmissionsByMonths = new List<GroupEmissionsByMonth>() { groupEmissionsByMonth };

            return animalGroupResults;
        }

        public GroupEmissionsByMonth GetTestGroupEmissionsByMonthForGrazingAnimals(FieldSystemComponent fieldSystemComponent)
        {
            var monthsAndDaysData = new MonthsAndDaysData();
            monthsAndDaysData.Year = DateTime.Now.Year;

            var managementPeriod = new ManagementPeriod();
            managementPeriod.HousingDetails = new HousingDetails();
            managementPeriod.HousingDetails.HousingType = HousingType.Pasture;
            managementPeriod.HousingDetails.PastureLocation = fieldSystemComponent;

            monthsAndDaysData.ManagementPeriod = managementPeriod;

            managementPeriod.ManureDetails.StateType = ManureStateType.Pasture;

            var groupEmissionsByDay = new GroupEmissionsByDay()
            {
                AdjustedAmountOfTanInStoredManureOnDay = 100,
                OrganicNitrogenCreatedOnDay = 50,
                TotalVolumeOfManureAvailableForLandApplication = 100,
                TotalCarbonUptakeForGroup = 100,
            };

            var groupEmissionsByMonth = new GroupEmissionsByMonth(monthsAndDaysData, new List<GroupEmissionsByDay>() { groupEmissionsByDay });

            return groupEmissionsByMonth;
        }

        public AnimalComponentEmissionsResults GetNonEmptyTestDairyCattleAnimalComponentEmissionsResults()
        {
            var results = new AnimalComponentEmissionsResults();
            results.Component = new DairyComponent();

            var monthsAndDaysData = new MonthsAndDaysData();
            monthsAndDaysData.Year = DateTime.Now.Year;

            var managementPeriod = new ManagementPeriod();
            managementPeriod.HousingDetails = new HousingDetails();
            monthsAndDaysData.ManagementPeriod = managementPeriod;

            var groupEmissionsByDay = new GroupEmissionsByDay()
            {
                AdjustedAmountOfTanInStoredManureOnDay = 200,
                OrganicNitrogenCreatedOnDay = 60,
                TotalVolumeOfManureAvailableForLandApplication = 500,
            };

            var groupEmissionsByMonth = new GroupEmissionsByMonth(monthsAndDaysData, new List<GroupEmissionsByDay>() { groupEmissionsByDay });

            var animalGroupResults = new AnimalGroupEmissionResults();
            animalGroupResults.GroupEmissionsByMonths = new List<GroupEmissionsByMonth>() { groupEmissionsByMonth };

            results.EmissionResultsForAllAnimalGroupsInComponent = new List<AnimalGroupEmissionResults>() { animalGroupResults };

            return results;
        }

        public GeographicData GetTestGeographicData()
        {
            var geographicData = new GeographicData();

            geographicData.SoilDataForAllComponentsWithinPolygon = new List<SoilData>();
            geographicData.SoilDataForAllComponentsWithinPolygon.Add(this.GetTestSoilData());

            geographicData.DefaultSoilData = geographicData.SoilDataForAllComponentsWithinPolygon.FirstOrDefault();

            return geographicData;
        }

        public FieldSystemComponent GetTestFieldComponent()
        {
            var component = new FieldSystemComponent();

            var viewItem = new CropViewItem();
            viewItem.CropType = CropType.Wheat;
            viewItem.Year = DateTime.Now.Year;

            component.CropViewItems.Add(viewItem);

            return component;
        }

        public CropViewItem GetTestCropViewItem()
        {
            var cropViewItem = new CropViewItem();
            cropViewItem.DateCreated = DateTime.Now;
            cropViewItem.CropType = CropType.Wheat;
            cropViewItem.Area = 1;
            cropViewItem.Year = DateTime.Now.Year;
            cropViewItem.Guid = Guid.NewGuid();

            cropViewItem.ManureApplicationViewItems = new ObservableCollection<ManureApplicationViewItem>();
            cropViewItem.ManureApplicationViewItems.Add(this.GetTestBeefCattleManureApplicationViewItemUsingOnLivestockManure());

            return cropViewItem;
        }

        public FertilizerApplicationViewItem GetTestFertilizerApplicationViewItem()
        {
            var fertilizerApplicationViewItem = new FertilizerApplicationViewItem();
            fertilizerApplicationViewItem.FertilizerBlendData =
                new Table_48_Carbon_Footprint_For_Fertilizer_Blends_Data()
                {
                    PercentageNitrogen = 50,
                };

            return fertilizerApplicationViewItem;
        }

        public SoilData GetTestSoilData( )
        {
            var soilData = new SoilData();
            soilData.EcodistrictId = 679;
            soilData.Province = Province.Alberta;
            soilData.PolygonId = 679001;
            soilData.SoilFunctionalCategory = SoilFunctionalCategory.Brown;
            soilData.SoilGreatGroup = SoilGreatGroupType.BlackChernozem;
            soilData.SoilTexture = SoilTexture.Fine;

            return soilData;
        }

        public ClimateData GetTestClimateData()
        {
            var climateData = new ClimateData();

            climateData.PrecipitationData = new PrecipitationData()
            {
                January = 10,
                February = 12,
                March = 10,
                April = 22,
                May = 2,
                June = 2,
                July = 8,
                August = 11,
                September = 17,
                October = 10,
                November = 8,
                December = 2,
            };

            climateData.TemperatureData = new TemperatureData()
            {
                January = 10,
                February = 12,
                March = 10,
                April = 22,
                May = 2,
                June = 2,
                July = 8,
                August = 11,
                September = 17,
                October = 10,
                November = 8,
                December = 2,
            };

            climateData.EvapotranspirationData = new EvapotranspirationData()
            {
                January = 10,
                February = 12,
                March = 10,
                April = 22,
                May = 2,
                June = 2,
                July = 8,
                August = 11,
                September = 17,
                October = 10,
                November = 8,
                December = 2,
            };

            return climateData;
        }

        public ManureApplicationViewItem GetTestBeefCattleManureApplicationViewItemUsingOnLivestockManure()
        {
            var manureApplicationViewItem = new ManureApplicationViewItem();
            manureApplicationViewItem.ManureLocationSourceType = ManureLocationSourceType.Livestock;
            manureApplicationViewItem.AnimalType = AnimalType.BeefBackgrounderHeifer;
            manureApplicationViewItem.AmountOfManureAppliedPerHectare = 50;
            manureApplicationViewItem.AmountOfNitrogenAppliedPerHectare = 100;

            return manureApplicationViewItem;
        }

        public ManureApplicationViewItem GetTestBeefCattleManureApplicationViewItemUsingImportedManure()
        {
            var manureApplicationViewItem = new ManureApplicationViewItem();
            manureApplicationViewItem.ManureLocationSourceType = ManureLocationSourceType.Imported;
            manureApplicationViewItem.AnimalType = AnimalType.BeefBackgrounderHeifer;
            manureApplicationViewItem.AmountOfManureAppliedPerHectare = 50;
            manureApplicationViewItem.AmountOfNitrogenAppliedPerHectare = 50;

            return manureApplicationViewItem;
        }

        public ManureApplicationViewItem GetTestDairyCattleManureApplicationViewItemUsingImportedManure()
        {
            var manureApplicationViewItem = new ManureApplicationViewItem();
            manureApplicationViewItem.ManureLocationSourceType = ManureLocationSourceType.Imported;
            manureApplicationViewItem.AnimalType = AnimalType.DairyHeifers;
            manureApplicationViewItem.AmountOfManureAppliedPerHectare = 333;
            manureApplicationViewItem.AmountOfNitrogenAppliedPerHectare = 50;

            return manureApplicationViewItem;
        }

        public ManureExportViewItem GetTestManureExportViewItem()
        {
            var manureExportViewItem = new ManureExportViewItem();
            manureExportViewItem.AnimalType = AnimalType.BeefBackgrounder;
            manureExportViewItem.DateOfExport = DateTime.Now;
            manureExportViewItem.Amount = 100;
            manureExportViewItem.DefaultManureCompositionData = new DefaultManureCompositionData()
            {
                NitrogenContent = 0.75,
            };

            return manureExportViewItem;
        }

        public Storage GetTestStorage()
        {
            var storage = new Storage();

            var farm = this.GetTestFarm();
            var applicationData = new ApplicationData();
            applicationData.GlobalSettings = new GlobalSettings();
            storage.ApplicationData = applicationData;

            storage.ApplicationData.Farms.Add(farm);
            
            applicationData.GlobalSettings.ActiveFarm = farm;
            ;
            return storage;
        }

        #endregion
    }
}