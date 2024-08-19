using H.Core.Models.LandManagement.Fields;

namespace H.Core.Calculators.Nitrogen
{
    public interface IIPCCNitrogenInputCalculator
    {
        /// <summary>
        /// Equation 2.7.2-1
        /// </summary>
        /// <returns></returns>
        double CalculateTotalAboveGroundResidueNitrogenUsingIpccTier2(
            double aboveGroundResidueDryMatter,
            double carbonConcentration,
            double nitrogenContentInStraw);

        /// <summary>
        /// Equation 2.7.2-2
        /// Equation 2.7.2-4
        /// Equation 2.6.2-6
        /// </summary>
        double CalculateTotalBelowGroundResidueNitrogenUsingIpccTier2(CropViewItem viewItem);
    }
}