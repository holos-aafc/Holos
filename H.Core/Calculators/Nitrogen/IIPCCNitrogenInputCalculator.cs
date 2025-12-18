using H.Core.Models.LandManagement.Fields;

namespace H.Core.Calculators.Nitrogen
{
    public interface IIPCCNitrogenInputCalculator
    {
        /// <summary>
        ///     Equation 2.7.2-1
        /// </summary>
        /// <returns></returns>
        double CalculateTotalAboveGroundResidueNitrogenUsingIpccTier2(
            double aboveGroundResidueDryMatterFromPreviousYear,
            double carbonConcentration,
            double nitrogenContentInStraw);

        /// <summary>
        ///     Equation 2.6.2-6
        /// </summary>
        double CalculateTotalBelowGroundResidueNitrogenUsingIpccTier2(CropViewItem currentYearViewItem,
            CropViewItem previousYearViewItem);

        /// <summary>
        ///     Equation 2.6.2-7
        /// </summary>
        /// <param name="aboveGroundBiomassExported">Above ground biomass (kg DM) exported (Equation 2.2.2-3)</param>
        /// <param name="nitrogenContentOfStraw">(kg kg^-1)</param>
        /// <returns>Total N from exported crop residues</returns>
        double CalculateTotalNitrogenFromExportedResidues(double aboveGroundBiomassExported,
            double nitrogenContentOfStraw);
    }
}