using System;
using H.Core.Calculators.Carbon;
using H.Core.Models.LandManagement.Fields;
using System.Collections.Generic;

namespace H.Core.Calculators.Nitrogen
{
    public class NitrogenInputCalculator : INitrogenCalculator
    {
        #region Fields

        private readonly IICBMNitrogenInputCalculator _icbmNitrogenInputCalculator;
        private readonly IIPCCNitrogenInputCalculator _ipccNitrogenInputCalculator;

        private readonly IIPCCTier2CarbonInputCalculator _ipccTier2CarbonInputCalculator;
        private readonly IICBMCarbonInputCalculator _icbmCarbonInputCalculator;

        #endregion

        #region Constructors

        public NitrogenInputCalculator(
            IICBMNitrogenInputCalculator icbmNitrogenInputCalculator, 
            IIPCCNitrogenInputCalculator ipccNitrogenInputCalculator, 
            IIPCCTier2CarbonInputCalculator ipccTier2CarbonInputCalculator, 
            IICBMCarbonInputCalculator icbmCarbonInputCalculator)
        {
            if (icbmNitrogenInputCalculator != null)
            {
                _icbmNitrogenInputCalculator = icbmNitrogenInputCalculator;
            }
            else
            {
                throw new ArgumentNullException(nameof(icbmNitrogenInputCalculator));
            }

            if (ipccNitrogenInputCalculator != null)
            {
                _ipccNitrogenInputCalculator = ipccNitrogenInputCalculator;
            }
            else
            {
                throw new ArgumentNullException(nameof(ipccNitrogenInputCalculator));
            }

            if (ipccTier2CarbonInputCalculator != null)
            {
                _ipccTier2CarbonInputCalculator = ipccTier2CarbonInputCalculator;
            }
            else
            {
                throw new ArgumentNullException(nameof(ipccTier2CarbonInputCalculator));
            }

            if (icbmCarbonInputCalculator != null)
            {
                _icbmCarbonInputCalculator = icbmCarbonInputCalculator;
            }
            else
            {
                throw new ArgumentNullException(nameof(icbmCarbonInputCalculator));
            }
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

        #endregion
    }
}