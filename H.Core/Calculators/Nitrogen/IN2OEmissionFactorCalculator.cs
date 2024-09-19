using H.Core.Models;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Calculators.Nitrogen
{
    public interface IN2OEmissionFactorCalculator
    {
        void Initialize(Farm farm);

        /// <summary>
        /// (kg N2O-N ha^-1)
        /// </summary>
        double CalculateTotalDirectN2ONFromExportedManure(Farm farm, ManureExportViewItem manureExportViewItem);
    }
}