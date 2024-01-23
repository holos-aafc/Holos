using H.Core.Calculators.Climate;
using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;
using H.Core.Models;
using H.Core.Providers.Climate;
using H.Core.Providers.Soil;
using H.Core.Providers;
using H.Core.Services.LandManagement;
using H.Core.Services;
using H.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using H.Core.Calculators.Carbon;
using H.Core.Calculators.Nitrogen;
using H.Core.Test;

namespace H.Integration
{
    [TestClass]
    public class N2O : UnitTestBase
    {
        #region Fields

        private Farm _farm;
        private Storage _storage;

        private static GeographicDataProvider _geographicDataProvider;
        
        private FieldResultsService _fieldResultsService;
        private CustomFileClimateDataProvider _customFileClimateDataProvider;
        private List<DailyClimateData> _lethbridgeDailyClimateData;
        private ClimateData _lethbridgeClimateData;
        private FieldComponentHelper _fieldComponentHelper;
        private Mock<IFarmResultsService> _mockFarmResultsService;
        

        #endregion

        #region Initialization

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _geographicDataProvider = new GeographicDataProvider();
            _geographicDataProvider.Initialize();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
        }

        [TestInitialize]
        public void TestInitialize()
        {
            var iCBMSoilCarbonCalculator = new ICBMSoilCarbonCalculator(_climateProvider, _n2OEmissionFactorCalculator);
            var n2oEmissionFactorCalculator = new N2OEmissionFactorCalculator(_climateProvider);
            var ipcc = new IPCCTier2SoilCarbonCalculator(_climateProvider, n2oEmissionFactorCalculator);

            var fieldResultsService = new FieldResultsService(iCBMSoilCarbonCalculator, ipcc, n2oEmissionFactorCalculator);

            _fieldResultsService = fieldResultsService;

            _storage = new Storage();
            _storage.ApplicationData = new ApplicationData();
            _storage.ApplicationData.GlobalSettings = new GlobalSettings();

            _farm = new Farm();
            _farm.Defaults = new Defaults();

            _storage.ApplicationData.GlobalSettings.ActiveFarm = _farm;

            _farm.StageStates.Add(new FieldSystemDetailsStageState());
            _mockFarmResultsService = new Mock<IFarmResultsService>();

            
            _customFileClimateDataProvider = new CustomFileClimateDataProvider();
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        [TestMethod]
        [Ignore]
        public void IntegrationTest()
        {
            _climateProvider = new ClimateProvider(new SlcClimateDataProvider());
            _fieldResultsService = new FieldResultsService(_iCbmSoilCarbonCalculator, _ipcc, _n2OEmissionFactorCalculator);

            var polygons = new List<int>() {825007}; //_geographicDataProvider.GetPolygonIdList();
            for (int i = 0; i < polygons.Count(); i++)
            {
                var polygon = polygons.ElementAt(i);

                _farm = new Farm();
                _farm.PolygonId = polygon;

                var geogrphicData = _geographicDataProvider.GetGeographicalData(polygon);
                _farm.GeographicData = geogrphicData;

                var climateData = _climateProvider.Get(polygon, _farm.Defaults.TimeFrame, _farm);
                _farm.ClimateData = climateData;

                var field = new FieldSystemComponent();
                field.StartYear = 1990;

                var crop = new CropViewItem();
                crop.CropType = CropType.TameLegume;
                field.CropViewItems.Add(crop);

                _fieldResultsService.AssignSystemDefaults(crop, _farm, _storage.ApplicationData.GlobalSettings);


                _farm.Components.Add(field);

                var fertilizerApplication = new FertilizerApplicationViewItem();
                fertilizerApplication.AmountOfNitrogenApplied = 150;
                crop.FertilizerApplicationViewItems.Add(fertilizerApplication);
                
                var manureApplication = new ManureApplicationViewItem();
                manureApplication.AmountOfNitrogenAppliedPerHectare = 500;
                crop.ManureApplicationViewItems.Add(manureApplication);

                _fieldResultsService.InitializeStageState(_farm);

                var finalResults = _fieldResultsService.CalculateFinalResults(_farm);

                //_fieldResultsService.ExportAllResultsToFile();
            }
        }
    }
}
