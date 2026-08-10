using System;
using H.Core.Calculators.Carbon;
using H.Core.Calculators.Nitrogen;
using H.Core.Models;
using H.Core.Providers.Climate;
using H.Core.Services.Initialization;

namespace H.Core.Services.LandManagement
{
    /// <summary>
    /// Assembles a fresh field-results calculation graph from the (stateless) climate provider and initialization service.
    /// The one N2O calculator is shared across the ICBM/IPCC soil-carbon calculators and the field results service - the
    /// same shared-instance wiring the composition roots use - but built anew on each <see cref="Create"/> so no per-run
    /// state can carry between farms.
    /// </summary>
    public class FieldResultsServiceFactory : IFieldResultsServiceFactory
    {
        private readonly IClimateProvider _climateProvider;
        private readonly IInitializationService _initializationService;

        public FieldResultsServiceFactory(IClimateProvider climateProvider, IInitializationService initializationService)
        {
            _climateProvider = climateProvider ?? throw new ArgumentNullException(nameof(climateProvider));
            _initializationService = initializationService ?? throw new ArgumentNullException(nameof(initializationService));
        }

        public FieldCalculationGraph Create(ManureTankStore sharedManureTankStore)
        {
            var n2OEmissionFactorCalculator = new N2OEmissionFactorCalculator(_climateProvider)
            {
                SharedManureTankStore = sharedManureTankStore,
            };
            var icbmSoilCarbonCalculator = new ICBMSoilCarbonCalculator(_climateProvider, n2OEmissionFactorCalculator);
            var ipccSoilCarbonCalculator = new IPCCTier2SoilCarbonCalculator(_climateProvider, n2OEmissionFactorCalculator);

            // The field service forwards the store to its carbon service and, in turn, the ICBM/IPCC carbon-input
            // calculators - so the whole field graph reads this run's shared tanks, set once at construction.
            var fieldResultsService = new FieldResultsService(
                icbmSoilCarbonCalculator, ipccSoilCarbonCalculator, n2OEmissionFactorCalculator, _initializationService)
            {
                SharedManureTankStore = sharedManureTankStore,
            };

            return new FieldCalculationGraph(fieldResultsService, n2OEmissionFactorCalculator);
        }
    }
}
