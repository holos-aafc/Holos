using System.Collections.Generic;
using H.Core.Emissions.Results;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Calculators.Nitrogen
{
    public interface IN2OEmissionFactorCalculator
    {
        ManureTankStore SharedManureTankStore { get; set; }

        /// <summary>
        /// Resets the per-run memoization caches so a reused calculator instance does not leak one farm's cached factors
        /// (or skip per-view-item reporting-field assignments) into the next farm. Call once at the start of each run.
        /// </summary>
        void ClearPerRunCaches();

        void Initialize(Farm farm);
        void Initialize(Farm farm, List<AnimalComponentEmissionsResults> animalResults);

        double CalculateTotalDirectN2ONFromExportedManure(Farm farm, ManureExportViewItem manureExportViewItem);

        double CalculateTotalIndirectN2ONFromExportedManure(Farm farm, int year);

        double CalculateTotalIndirectN2ONFromExportedManure(Farm farm, ManureExportViewItem manureExportViewItem);

        double CalculateTotalNitrateLeachedFromExportedManure(Farm farm, ManureExportViewItem manureExportViewItem);

        double CalculateTotalNitrateLeachedFromExportedManureForFarmAndYear(Farm farm, int year);

        double CalculateVolatilizationEmissionsFromExportedManure(Farm farm, ManureExportViewItem manureExportViewItem);

        /// <summary>
        /// (kg NH3-N)
        /// </summary>
        double CalculateAdjustedNH3NLossFromManureExports(Farm farm, int year, ManureExportViewItem manureExportViewItem);

        /// <summary>
        /// (kg NH3-N)
        /// </summary>
        double CalculateAdjustedNH3NLossFromManureExports(Farm farm, ManureExportViewItem manureExportViewItem);
    }
}