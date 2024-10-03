using System;
using H.Core.Calculators.Carbon;
using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Calculators.Nitrogen
{
    /// <summary>
    /// A service class to calculate inputs related to N. This will route calculations to the IPCC Tier 2 or ICBM methodology as required
    /// </summary>
    public class NitrogenService : INitrogenCalculator
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

        public double CalculateAboveGroundResidueNitrogen(CropViewItem cropViewItem)
        {
            if (this.CanCalculateNitrogenInputsUsingIpccTier2(cropViewItem))
            {
                return _ipccNitrogenInputCalculator.CalculateTotalAboveGroundResidueNitrogenUsingIpccTier2(
                    cropViewItem.AboveGroundResidueDryMatter,
                    cropViewItem.CarbonConcentration,
                    cropViewItem.NitrogenContentInStraw);
            }
            else
            {
                return _icbmNitrogenInputCalculator.CalculateTotalAboveGroundResidueNitrogenUsingIcbm(cropViewItem);
            }
        }

        public double CalculateBelowGroundResidueNitrogen(CropViewItem cropViewItem)
        {
            if (this.CanCalculateNitrogenInputsUsingIpccTier2(cropViewItem))
            {
                return _ipccNitrogenInputCalculator.CalculateTotalBelowGroundResidueNitrogenUsingIpccTier2(cropViewItem);
            }
            else
            {
                return _icbmNitrogenInputCalculator.CalculateTotalBelowGroundResidueNitrogenUsingIcbm(cropViewItem);
            }
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