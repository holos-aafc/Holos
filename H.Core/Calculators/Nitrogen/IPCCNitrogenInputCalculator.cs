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
        /// Equation 2.5.6-8
        /// Equation 2.7.2-2
        /// </summary>
        public double CalculateTotalBelowGroundResidueNitrogenUsingIpccTier2(CropViewItem viewItem)
        {
            if (viewItem.CropType.IsPerennial())
            {
                var rootNitrogen = this.CalculateNitrogenContentRootReturnedToSoil(
                    carbonInputFromRoots: viewItem.CarbonInputFromRoots,
                    nitrogenConcentrationInRoots: viewItem.NitrogenContentInRoots);

                var exudateNitrogen = this.CalculateNitrogenContentExaduatesReturnedToSoil(
                    carbonInputFromExtraroots: viewItem.CarbonInputFromExtraroots,
                    nitrogenConcentrationInExtraroots: viewItem.NitrogenContentInExtraroot);

                // Equation 2.5.6-8
                var perennialNitrogen = rootNitrogen + exudateNitrogen;

                return perennialNitrogen;
            }

            // Equation 2.7.2-2
            var result = (viewItem.BelowGroundResidueDryMatter / viewItem.Area) * viewItem.NitrogenContentInRoots;

            return result;
        }

        /// <summary>
        /// Equation 2.6.2-8
        /// </summary>
        /// <param name="aboveGroundBiomassExported">Above ground biomass (kg DM) exported (Equation 2.2.2-3)</param>
        /// <param name="nitrogenContentOfStraw">(kg kg^-1)</param>
        /// <returns>Total N from exported crop residues</returns>
        public double CalculateTotalNitrogenFromExportedResidues(double aboveGroundBiomassExported, double nitrogenContentOfStraw)
        {
            return aboveGroundBiomassExported * nitrogenContentOfStraw;
        }

        #endregion
    }
}