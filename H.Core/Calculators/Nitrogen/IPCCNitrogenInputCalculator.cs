using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Calculators.Nitrogen
{
    public class IPCCNitrogenInputCalculator : NitrogenInputCalculatorBase, IIPCCNitrogenInputCalculator
    {
        #region Public Methods

        /// <summary>
        /// Equation 2.7.2-1
        /// </summary>
        /// <returns></returns>
        public double CalculateTotalAboveGroundResidueNitrogenUsingIpccTier2(
            double aboveGroundResidueDryMatter,
            double carbonConcentration,
            double nitrogenContentInStraw)
        {
            return aboveGroundResidueDryMatter * nitrogenContentInStraw;
        }

        /// <summary>
        /// Equation 2.7.2-2
        /// Equation 2.7.2-4
        /// Equation 2.6.2-6
        /// </summary>
        public double CalculateTotalBelowGroundResidueNitrogenUsingIpccTier2(CropViewItem viewItem)
        {
            if (viewItem.CropType.IsPerennial())
            {
                var perennialNitrogen = (viewItem.BelowGroundResidueDryMatter / viewItem.Area) * viewItem.NitrogenContentInRoots * (viewItem.PercentageOfRootsReturnedToSoil / 100.0);

                return perennialNitrogen;
            }
            // Equation 2.7.2-2
            var result = (viewItem.BelowGroundResidueDryMatter / viewItem.Area) * viewItem.NitrogenContentInRoots;

            return result;
        }

        #endregion
    }
}