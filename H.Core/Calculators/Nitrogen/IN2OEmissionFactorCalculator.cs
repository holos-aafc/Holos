using System.Collections.Generic;
using H.Core.Emissions.Results;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Calculators.Nitrogen
{
    public interface IN2OEmissionFactorCalculator
    {
        void Initialize(Farm farm);
        void Initialize(Farm farm, List<AnimalComponentEmissionsResults> animalResults);

        /// <summary>
        /// (kg N2O-N ha^-1)
        /// </summary>
        double CalculateTotalDirectN2ONFromExportedManure(Farm farm, ManureExportViewItem manureExportViewItem);
    }
}