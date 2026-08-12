using H.Core.Calculators.Nitrogen;
using H.Core.Models;

namespace H.Core.Services.LandManagement
{
    /// <summary>
    /// Builds a FRESH field-results calculation graph for a single farm run. The soil-carbon calculators (ICBM/IPCC)
    /// accumulate N-pool state and the N2O calculator memoizes per run, so reusing one instance across farms leaks state
    /// between runs (the GUI and CLI both run every farm through one pipeline). A fresh graph per run eliminates that whole
    /// class of inter-run state by construction (issue #451 follow-up / "Option 2" scoped composition).
    /// </summary>
    public interface IFieldResultsServiceFactory
    {
        /// <summary>
        /// Builds the graph with this run's manure-tank store injected into the calculators (so the field / indirect-N2O /
        /// soil-carbon path reads the same tanks). Pass null for an idle graph that owns private tanks.
        /// </summary>
        FieldCalculationGraph Create(ManureTankStore sharedManureTankStore);
    }

    /// <summary>
    /// A freshly built field-results service plus the N2O calculator it shares internally, handed back together so the
    /// caller uses the same fresh N2O instance for its own (manure-export) calculations.
    /// </summary>
    public class FieldCalculationGraph
    {
        public FieldCalculationGraph(IFieldResultsService fieldResultsService, IN2OEmissionFactorCalculator n2OEmissionFactorCalculator)
        {
            this.FieldResultsService = fieldResultsService;
            this.N2OEmissionFactorCalculator = n2OEmissionFactorCalculator;
        }

        public IFieldResultsService FieldResultsService { get; }

        public IN2OEmissionFactorCalculator N2OEmissionFactorCalculator { get; }
    }
}
