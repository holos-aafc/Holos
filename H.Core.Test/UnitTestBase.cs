using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.Animals.Beef;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models.Animals.Dairy;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Animals;
using H.Core.Providers.Climate;
using H.Core.Services;
using H.Core.Services.Animals;
using Moq;

namespace H.Core.Test
{
    public abstract class UnitTestBase
    {
        #region Fields

        protected Mock<IFarmResultsService> _mockFarmResultService;
        protected IFarmResultsService _mockFarmResultServiceObject;
        protected Mock<IManureService> _mockManureService;
        protected IManureService _mockManureServiceObject;
        protected Mock<IClimateProvider> _mockClimateProvider;
        protected IClimateProvider _mockClimateProviderObject;
        protected Mock<IAnimalEmissionFactorsProvider> _mockEmissionDataProvider;
        protected IAnimalEmissionFactorsProvider _mockEmissionDataProviderObject;
        protected Mock<IAnimalAmmoniaEmissionFactorProvider> _mockAnimalAmmoniaEmissionFactorProvider;
        protected IAnimalAmmoniaEmissionFactorProvider _mockAnimalAmmoniaEmissionFactorProviderObject;

        #endregion

        #region Constructors

        protected UnitTestBase()
        {
            _mockFarmResultService = new Mock<IFarmResultsService>();
            _mockFarmResultServiceObject = _mockFarmResultService.Object;

            _mockManureService = new Mock<IManureService>();
            _mockManureServiceObject = _mockManureService.Object;

            _mockClimateProvider = new Mock<IClimateProvider>();
            _mockClimateProviderObject = _mockClimateProvider.Object;

            _mockEmissionDataProvider = new Mock<IAnimalEmissionFactorsProvider>();
            _mockEmissionDataProviderObject = _mockEmissionDataProvider.Object;

            _mockAnimalAmmoniaEmissionFactorProvider = new Mock<IAnimalAmmoniaEmissionFactorProvider>();
            _mockAnimalAmmoniaEmissionFactorProviderObject = _mockAnimalAmmoniaEmissionFactorProvider.Object;
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
            managementPeriod.Start = DateTime.Now;
            managementPeriod.End = managementPeriod.Start.AddDays(30 * 2);

            managementPeriod.ManureDetails.StateType = ManureStateType.AnaerobicDigester;

            farm.Components.Add(backgroundingComponent);
            farm.Components.Add(dairyComponent);

            backgroundingComponent.Groups.Add(group);
            dairyComponent.Groups.Add(cowsGroup);

            group.ManagementPeriods.Add(managementPeriod);

            /*
             * Manure exports
             */

            farm.ManureExportViewItems.Add(new ManureExportViewItem() { DateOfExport = DateTime.Now, Amount = 1000, AnimalType = AnimalType.Dairy });
            farm.ManureExportViewItems.Add(new ManureExportViewItem() { DateOfExport = DateTime.Now, Amount = 2000, AnimalType = AnimalType.Dairy });

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

        public AnimalComponentEmissionsResults GetNonEmptyTestBeefCattleAnimalComponentEmissionsResults()
        {
            var results = new AnimalComponentEmissionsResults();
            results.Component = new BackgroundingComponent();

            var monthsAndDaysData = new MonthsAndDaysData();
            monthsAndDaysData.Year = DateTime.Now.Year;

            var managementPeriod = new ManagementPeriod();
            managementPeriod.HousingDetails = new HousingDetails();
            monthsAndDaysData.ManagementPeriod = managementPeriod;

            var groupEmissionsByDay = new GroupEmissionsByDay()
            {
                AdjustedAmountOfTanInStoredManureOnDay = 100,
                OrganicNitrogenCreatedOnDay = 50,
                TotalVolumeOfManureAvailableForLandApplication = 100,
            };

            var groupEmissionsByMonth = new GroupEmissionsByMonth(monthsAndDaysData, new List<GroupEmissionsByDay>() { groupEmissionsByDay });

            var animalGroupResults = new AnimalGroupEmissionResults();
            animalGroupResults.GroupEmissionsByMonths = new List<GroupEmissionsByMonth>() { groupEmissionsByMonth };

            results.EmissionResultsForAllAnimalGroupsInComponent = new List<AnimalGroupEmissionResults>() { animalGroupResults };

            return results;
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
            cropViewItem.Area = 1;

            cropViewItem.ManureApplicationViewItems = new ObservableCollection<ManureApplicationViewItem>();
            cropViewItem.ManureApplicationViewItems.Add(this.GetTestBeefCattleManureApplicationViewItemUsingOnLivestockManure());

            return cropViewItem;
        }

        public ManureApplicationViewItem GetTestBeefCattleManureApplicationViewItemUsingOnLivestockManure()
        {
            var manureApplicationViewItem = new ManureApplicationViewItem();
            manureApplicationViewItem.ManureLocationSourceType = ManureLocationSourceType.Livestock;
            manureApplicationViewItem.AnimalType = AnimalType.BeefBackgrounderHeifer;
            manureApplicationViewItem.AmountOfManureAppliedPerHectare = 50;

            return manureApplicationViewItem;
        }

        public ManureApplicationViewItem GetTestBeefCattleManureApplicationViewItemUsingImportedManure()
        {
            var manureApplicationViewItem = new ManureApplicationViewItem();
            manureApplicationViewItem.ManureLocationSourceType = ManureLocationSourceType.Imported;
            manureApplicationViewItem.AnimalType = AnimalType.BeefBackgrounderHeifer;
            manureApplicationViewItem.AmountOfManureAppliedPerHectare = 50;

            return manureApplicationViewItem;
        }

        public ManureApplicationViewItem GetTestDairyCattleManureApplicationViewItemUsingImportedManure()
        {
            var manureApplicationViewItem = new ManureApplicationViewItem();
            manureApplicationViewItem.ManureLocationSourceType = ManureLocationSourceType.Imported;
            manureApplicationViewItem.AnimalType = AnimalType.DairyHeifers;
            manureApplicationViewItem.AmountOfManureAppliedPerHectare = 333;

            return manureApplicationViewItem;
        }

        #endregion
    }
}