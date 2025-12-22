using H.CLI.Processors;
using H.Core.Calculators.Carbon;
using H.Core.Calculators.Nitrogen;
using H.Core.Providers.Climate;
using H.Core.Providers;
using H.Core.Services.Initialization;
using H.Core.Services.LandManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using H.CLI.Factories;
using H.CLI.UserInput;

namespace H.CLI.Test.Processors
{
    [TestClass]
    public class ProccessorHandlerTest
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
        private static ProcessorHandler _processorHandler;

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
            _processorHandler = new ProcessorHandler(_fieldResultsService);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {

        }

        #endregion

        [TestMethod]
        public void TestSetProcessor()
        {
            _processorHandler.SetProccessor(new ShelterbeltProcessor());
            Assert.IsInstanceOfType(_processorHandler._processor, typeof(ShelterbeltProcessor));
        }
    }
}
