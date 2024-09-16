using H.Core.Calculators.Carbon;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Calculators.Nitrogen
{
    public class NitrogenCalculator : INitrogenCalculator
    {
        #region Fields

        private IIPCCTier2CarbonInputCalculator _ipccTier2CarbonInputCalculator;
        private readonly IICBMNitrogenInputCalculator _icbmNitrogenInputCalculator;
        private readonly IIPCCNitrogenInputCalculator _ipccNitrogenInputCalculator;

        #endregion

        #region Constructors

        public NitrogenCalculator()
        {
            _ipccTier2CarbonInputCalculator = new IPCCTier2CarbonInputCalculator();
            _icbmNitrogenInputCalculator = new ICBMNitrogenInputCalculator();
            _ipccNitrogenInputCalculator = new IPCCNitrogenInputCalculator();
        }

        #endregion

        #region Public Methods

        public double CalculateAboveGroundResidueNitrogen(CropViewItem cropViewItem)
        {
            if (_ipccTier2CarbonInputCalculator.CanCalculateInputsForCrop(cropViewItem))
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
            if (_ipccTier2CarbonInputCalculator.CanCalculateInputsForCrop(cropViewItem))
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
            return _ipccNitrogenInputCalculator.CalculateTotalNitrogenFromExportedResidues(cropViewItem.AboveGroundResidueDryMatterExported, cropViewItem.NitrogenContentInStraw);
        }

        #endregion
    }
}