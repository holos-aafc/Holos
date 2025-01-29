using System;
using H.Core.Calculators.Carbon;
using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Calculators.Nitrogen
{
    /// <summary>
    /// A service class to calculate inputs related to N. This will route calculations to the IPCC Tier 2 or ICBM methodology as required
    /// </summary>
    public class NitrogenService : INitrogenService
    {
        #region Fields

        private readonly ICarbonService _carbonService;

        private readonly IICBMNitrogenInputCalculator _icbmNitrogenInputCalculator;
        private readonly IIPCCNitrogenInputCalculator _ipccNitrogenInputCalculator;

        #endregion

        #region Constructors

        public NitrogenService()
        {
            _carbonService = new CarbonService();

            _icbmNitrogenInputCalculator = new ICBMNitrogenInputCalculator();
            _ipccNitrogenInputCalculator = new IPCCNitrogenInputCalculator();
        }

        #endregion

        #region Public Methods

        public double CalculateAboveGroundResidueNitrogen(
            CropViewItem currentYearViewItem,
            CropViewItem previousYearViewItem)
        {
            if (this.CanCalculateNitrogenInputsUsingIpccTier2(currentYearViewItem))
            {
                var aboveGroundResidueDryMatterFromPreviousYear = previousYearViewItem != null ? previousYearViewItem.AboveGroundResidueDryMatter : 0;

                return _ipccNitrogenInputCalculator.CalculateTotalAboveGroundResidueNitrogenUsingIpccTier2(
                    aboveGroundResidueDryMatterFromPreviousYear,
                    currentYearViewItem.CarbonConcentration,
                    currentYearViewItem.NitrogenContentInStraw);
            }
            else
            {
                return _icbmNitrogenInputCalculator.CalculateTotalAboveGroundResidueNitrogenUsingIcbm(currentYearViewItem, previousYearViewItem);
            }
        }

        public double CalculateBelowGroundResidueNitrogen(
            CropViewItem currentYearViewItem,
            CropViewItem previousYearViewItem)
        {
            if (this.CanCalculateNitrogenInputsUsingIpccTier2(currentYearViewItem))
            {
                return _ipccNitrogenInputCalculator.CalculateTotalBelowGroundResidueNitrogenUsingIpccTier2(currentYearViewItem, previousYearViewItem);
            }
            else
            {
                return _icbmNitrogenInputCalculator.CalculateTotalBelowGroundResidueNitrogenUsingIcbm(currentYearViewItem, previousYearViewItem);
            }
        }

        public double CalculateCropResidueExportNitrogen(CropViewItem cropViewItem, HarvestViewItem harvestViewItem)
        {
            var result = 0d;

            return result;
        }

        public double CalculateCropResidueExportNitrogen(CropViewItem cropViewItem)
        {
            if (cropViewItem.CropType.IsPerennial())
            {
                // Convert to per ha
                var dryMatterPerHectare = cropViewItem.TotalDryMatterLostFromBaleExports / cropViewItem.Area;

                // Use N content in product since N content in straw is 0 for perennials

                // Use IPCC Tier 2 for now until there is a method in algorithm document for ICBM approach
                return _ipccNitrogenInputCalculator.CalculateTotalNitrogenFromExportedResidues(dryMatterPerHectare, cropViewItem.NitrogenContentInProduct);
            }
            else
            {
                // Use IPCC Tier 2
                return _ipccNitrogenInputCalculator.CalculateTotalNitrogenFromExportedResidues(cropViewItem.AboveGroundResidueDryMatterExported, cropViewItem.NitrogenContentInStraw);
            }
        }

        #endregion

        #region Private Methods

        private bool CanCalculateNitrogenInputsUsingIpccTier2(CropViewItem cropViewItem)
        {
            return _carbonService.CanCalculateInputsUsingIpccTier2(cropViewItem);
        }

        #endregion
    }
}