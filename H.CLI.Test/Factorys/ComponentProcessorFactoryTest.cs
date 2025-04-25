using H.CLI.Factories;
using H.CLI.Processors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Models.LandManagement.Shelterbelt;
using H.Core.Providers.Climate;
using H.Core.Providers;
using H.CLI.UserInput;
using System.Globalization;
using H.CLI.FileAndDirectoryAccessors;
using H.CLI.Handlers;
using H.Core.Calculators.Carbon;
using H.Core.Calculators.Nitrogen;
using H.Core.Services.Initialization;
using H.Core.Services.LandManagement;

namespace H.CLI.Test.Factorys
{
    [TestClass]
    public class ComponentProcessorFactoryTest
    {
        #region Fields

        private static ComponentProcessorFactory _componentProcessorFactory;
        private static N2OEmissionFactorCalculator _n2OEmissionFactorCalculator;
        private static ICBMSoilCarbonCalculator _iCbmSoilCarbonCalculator;
        private static IPCCTier2SoilCarbonCalculator _ipccSoilCarbonCalculator;
        private static InitializationService _initializationService;
        private static FieldResultsService _fieldResultsService;
        public static readonly GeographicDataProvider geographicDataProvider = new GeographicDataProvider();
        private static ClimateProvider _climateProvider;
        private static SlcClimateDataProvider _slcClimateDataProvider;

        #endregion

        #region Initialization

        [ClassInitialize]
        public static void ClassInitialzie(TestContext context)
        {
            _slcClimateDataProvider = new SlcClimateDataProvider();
            _climateProvider = new ClimateProvider(_slcClimateDataProvider);
            _n2OEmissionFactorCalculator = new N2OEmissionFactorCalculator(_climateProvider);
            _iCbmSoilCarbonCalculator = new ICBMSoilCarbonCalculator(_climateProvider, _n2OEmissionFactorCalculator);
            _ipccSoilCarbonCalculator = new IPCCTier2SoilCarbonCalculator(_climateProvider, _n2OEmissionFactorCalculator);
            _initializationService = new InitializationService();
            _fieldResultsService = new FieldResultsService(_iCbmSoilCarbonCalculator, _ipccSoilCarbonCalculator, _n2OEmissionFactorCalculator, _initializationService);
            geographicDataProvider.Initialize();
            CLILanguageConstants.SetCulture(CultureInfo.CreateSpecificCulture("en-CA"));
            _slcClimateDataProvider = new SlcClimateDataProvider();
            _climateProvider = new ClimateProvider(_slcClimateDataProvider);
            _componentProcessorFactory = new ComponentProcessorFactory(_fieldResultsService);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {

        }

        #endregion

        [TestMethod]
        public void TestComponentProcessorFactory()
        {
            var result = _componentProcessorFactory.GetComponentProcessor(new ShelterbeltComponent().GetType());
            Assert.IsInstanceOfType(result, typeof(ShelterbeltProcessor));

        }

    }
}
