using H.CLI.Interfaces;
using H.CLI.Processors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Services.LandManagement;
using H.Core.Calculators.Carbon;
using H.Core.Calculators.Nitrogen;
using H.Core.Providers;
using H.Core.Providers.Climate;
using H.Core.Services;

namespace H.CLI.Factories
{
    public class ComponentProcessorFactory
    {
        #region Fields

        private readonly FieldProcessor _fieldProcessor;
        private readonly ShelterbeltProcessor _shelterbeltProcessor;

        #endregion

        #region Constructors

        public ComponentProcessorFactory()
        {
            var climateProvider = new ClimateProvider(new SlcClimateDataProvider());
            var n2oEmissionFactorCalculator = new N2OEmissionFactorCalculator(climateProvider);
            var iCBMSoilCarbonCalculator = new ICBMSoilCarbonCalculator(climateProvider, n2oEmissionFactorCalculator);
            var ipcc = new IPCCTier2SoilCarbonCalculator(climateProvider, n2oEmissionFactorCalculator);
            var initializationService = new InitializationService();

            var fieldResultsService = new FieldResultsService(iCBMSoilCarbonCalculator, ipcc, n2oEmissionFactorCalculator, initializationService);
            _fieldProcessor = new FieldProcessor(fieldResultsService);
            _shelterbeltProcessor = new ShelterbeltProcessor();
        } 

        #endregion

        #region Public Methods
        /// <summary>
        /// Based on the type of Component in our farm's list of components, return the appropriate concrete Processor
        /// </summary>
        /// <param name="componentType"></param>
        /// <returns></returns>
        public IProcessor GetComponentProcessor(Type componentType)
        {
            switch (componentType.Name.ToUpper())
            {
                case "SHELTERBELTCOMPONENT":
                    return _shelterbeltProcessor;
                case "FIELDSYSTEMCOMPONENT":
                    return _fieldProcessor;
                default:
                    return new ShelterbeltProcessor();
            }
        } 
        #endregion
    }
}
